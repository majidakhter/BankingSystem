using AutoMapper;
using BankingApp.AccountManagement.Application.Transfer.Models;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.AccountManagement.Application.Transfer.Queries
{
    public sealed record GetTransactionByTransactionIdQuery(int transactionNo) : Query<List<TransferDTO>>;
    public sealed class GetTransactionByTransactionIdQueryHandler : QueryHandler<GetTransactionByTransactionIdQuery, List<TransferDTO>>
    {

        private readonly IRepository<Credit> _creditrepository;
        private readonly IRepository<Debit> _debitrepository;
        public GetTransactionByTransactionIdQueryHandler(IMapper mapper,
             IRepository<Credit> creditrepository, IRepository<Debit> debitrepository) : base(mapper)
        {
            _creditrepository = creditrepository;
            _debitrepository = debitrepository;
        }

        protected override async Task<List<TransferDTO>> HandleAsync(GetTransactionByTransactionIdQuery request)
        {
            var credits = await _creditrepository.FetchMulti(x=>x.TransactionNo == request.transactionNo).ToListAsync();
            var debits = await _debitrepository.FetchMulti(x => x.TransactionNo == request.transactionNo).ToListAsync();

            //TODO it will return single record ecause transactionid always unique so instead of List<TransferDTO> use TransferDTO
            // 1. Combine and flatten the tables using a Full Outer Join approach
            var query = debits.Select(d => new TransferDTO{
                    TransactionDate = d.TransactionDate,
                    TransactionNo = d.TransactionNo,
                    Description = d.Description,
                    DebitAmount = d.Amount.Value,
                    CreditAmount = 0m
                })
                .Concat(
                    credits
                        .Select(c => new TransferDTO
                        {
                            TransactionDate = c.TransactionDate,
                            TransactionNo = c.TransactionNo,
                            Description = c.Description,
                            DebitAmount = 0m,
                            CreditAmount = c.Amount.Value,
                        })
                )
                .OrderBy(x => x.TransactionDate)
                .ToList();

            // Calculate running balance iteratively
            decimal runningBalance = 0m;
            var finalResult = query.Select(x => {
                runningBalance += (x.DebitAmount!.Value - x.CreditAmount!.Value);
                return new TransferDTO
                {
                    TransactionDate = x.TransactionDate,
                    TransactionNo = x.TransactionNo,
                    Description = x.Description,
                    DebitAmount = x.DebitAmount,
                    CreditAmount = x.CreditAmount,
                    Balance = runningBalance
                };
            }).ToList();
            return finalResult;
        }
    }
}
