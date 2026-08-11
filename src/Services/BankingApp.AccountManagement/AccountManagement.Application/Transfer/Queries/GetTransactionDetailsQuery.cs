using AutoMapper;
using BankingApp.AccountManagement.Application.Transfer.Models;
using BankingAppDDD.Applications.Abstractions.Queries;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Accounts.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.AccountManagement.Application.Transfer.Queries
{
    public sealed record GetTransactionDetailsQuery(Guid accountId, DateTime startDate, DateTime endDate) : Query<List<TransferDTO>>;
    public sealed class GetTransactionDetailsQueryHandler : QueryHandler<GetTransactionDetailsQuery, List<TransferDTO>>
    {
        private readonly IRepository<Credit> _creditrepository;
        private readonly IRepository<Debit> _debitrepository;
        public GetTransactionDetailsQueryHandler(IMapper mapper,
            IRepository<Credit> creditrepository, IRepository<Debit> debitrepository) : base(mapper)
        {
            _creditrepository = creditrepository;
            _debitrepository = debitrepository;
        }

        protected override async Task<List<TransferDTO>> HandleAsync(GetTransactionDetailsQuery request)
        {
            var credits = await _creditrepository.GetAll().ToListAsync();
            var debits = await _debitrepository.GetAll().ToListAsync();

            // 1. Combine and flatten the tables using a Full Outer Join approach
            var combinedData = (from c in credits.Where(c => c.AccountId == request.accountId)
                                join d in debits.Where(d => d.AccountId == request.accountId)
                                on c.TransactionNo equals d.TransactionNo into temp
                                from d in temp.DefaultIfEmpty()
                                select new { c, d })
                               .Union(
                                from d in debits.Where(d => d.AccountId == request.accountId)
                                join c in credits.Where(c => c.AccountId == request.accountId)
                                on d.TransactionNo equals c.TransactionNo into temp
                                from c in temp.DefaultIfEmpty()
                                select new { c, d })
                               .Distinct()
                               .Select(x => new 
                               {
                                   TransactionDate = x.c != null ? x.c.TransactionDate : x.d.TransactionDate,
                                   TransactionNo = x.c != null ? x.c.TransactionNo : x.d.TransactionNo,
                                   Description = x.c != null ? x.c.Description : x.d.Description,
                                   DebitAmount = x.d != null ? x.d.Amount.Value : 0m,
                                   CreditAmount = x.c != null ? x.c.Amount.Value : 0m
                               })
                               .OrderBy(x => x.TransactionDate)
                               .ToList();

            decimal runningBalance = 0;
            var finalResult = combinedData.Select(x => {
                runningBalance += (x.DebitAmount - x.CreditAmount);
                return new TransferDTO
                {
                    TransactionDate = x.TransactionDate,
                    TransactionNo = x.TransactionNo,
                    Description = x.Description,
                    DebitAmount = x.DebitAmount,
                    CreditAmount = x.CreditAmount,
                    Balance = runningBalance,
                };
            }).ToList();
            var dateRangeResult = finalResult.Where(x => x.TransactionDate >= request.startDate && x.TransactionDate < request.endDate);
            return dateRangeResult.ToList();
        }
    }
}
