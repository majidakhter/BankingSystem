using BankingApp.LoanManagement.Core.DebtInfos.Entities;
using BankingApp.LoanManagement.Core.DebtInfos.ValueObjects;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;

namespace BankingApp.LoanManagement.Application.DebtorInfosCommand
{
    public sealed record CreateDebtorInfoCommand(Guid customerId, List<decimal> amount) : CreateCommand;

    public sealed class CreateDebtorInfoCommandHandler : CreateCommandHandler<CreateDebtorInfoCommand>
    {
        private readonly ILoanRepository<DebtorInfo> _repository;
        public CreateDebtorInfoCommandHandler(ILoanRepository<DebtorInfo> repository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
        }
        protected override async Task<Guid> HandleAsync(CreateDebtorInfoCommand request)
        {
            List<Debt> amounts = new List<Debt>();
            foreach (var item in request.amount)
            {
                amounts.Add(Debt.Create(item));
            }
            var created = DebtorInfo.Create(request.customerId, amounts);
            _repository.Insert(created);
            await UnitOfWork.CommitAsync();
            return created.Id;
        }
    }
}

