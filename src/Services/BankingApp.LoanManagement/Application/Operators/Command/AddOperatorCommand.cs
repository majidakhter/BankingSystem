using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;

namespace BankingApp.LoanManagement.Application.OperatorsCommand
{
    public sealed record AddOperatorCommand(decimal CompetenceLevelAmount) : CreateCommand;

    public sealed class AddOperatorCommandHandler : CreateCommandHandler<AddOperatorCommand>
    {
        private readonly ILoanRepository<Operator> _repository;
        public AddOperatorCommandHandler(ILoanRepository<Operator> repository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
        }

        protected override async Task<Guid> HandleAsync(AddOperatorCommand request)
        {

            var created = Operator.Create(request.CompetenceLevelAmount, Guid.Empty);
            //Guard.Against.NotFound(created);
            _repository.Insert(created);
            await UnitOfWork.CommitAsync();
            return created.Id;
        }
    }
}
