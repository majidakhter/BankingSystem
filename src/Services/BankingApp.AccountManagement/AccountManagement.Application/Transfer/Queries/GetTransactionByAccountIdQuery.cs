using AutoMapper;
using BankingApp.AccountManagement.Application.Transfer.Models;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using BankingAppDDD.Domains.Accounts.Entities;

namespace BankingApp.AccountManagement.Application.Transfer.Queries
{
    public sealed record GetTransactionByAccountIdQuery(Guid AccountId) : Query<List<TransferDTO>>;
    public class GetTransactionByAccountIdQueryHandler : QueryHandler<GetTransactionByAccountIdQuery, List<TransferDTO>>
    {
        private readonly IRepository<Credit> _creditrepository;
        private readonly IRepository<Debit> _debitrepository;
        public GetTransactionByAccountIdQueryHandler(IMapper mapper,
             IRepository<Credit> creditrepository, IRepository<Debit> debitrepository) : base(mapper)
        {
            _creditrepository = creditrepository;
            _debitrepository = debitrepository;
        }
        protected override async Task<List<TransferDTO>> HandleAsync(GetTransactionByAccountIdQuery request)
        {
            var creditlist = await _creditrepository.FetchMulti(x=>x.AccountId ==request.AccountId).ToListAsync();
            var debitList = await _debitrepository.FetchMulti(x => x.AccountId == request.AccountId).ToListAsync();

            // 1. Fetch and filter data from both tables
            var credits = creditlist.Select(c => new TransferDTO{
                TransactionDate = c.TransactionDate,
                TransactionNo = c.TransactionNo,
                Description = c.Description,
                DebitAmount = 0m,
                CreditAmount = c.Amount.Value
            });
            var debits = debitList.Select(d => new TransferDTO
            {
                TransactionDate = d.TransactionDate,
                TransactionNo = d.TransactionNo,
                Description = d.Description,
                DebitAmount = d.Amount.Value,
                CreditAmount = 0m
            });

            // 2. Combine and sort the data chronologically
            var combinedTransactions = credits
                .Concat(debits)
                .OrderBy(x => x.TransactionDate)
                .ToList();

            // 3. Compute running balance sequentially
           
            

            decimal runningBalance = 0m;
            var finalResult = combinedTransactions.Select(x => {
                runningBalance += (x.CreditAmount!.Value - x.DebitAmount!.Value);
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
