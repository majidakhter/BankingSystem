using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingApp.LoanManagement.Application.LoanApplicationCommands
{
    public sealed record LoanApplicationSubmittedCommand(Guid operatorId, LoanApplicationData loanData) : CreateCommand;
    public sealed class LoanApplicationSubmittedCommandHandler : CreateCommandHandler<LoanApplicationSubmittedCommand>
    {
        private readonly ILoanRepository<LoanApplication> _repository;
        private readonly ILoanRepository<Operator> _operatorrepository;
        public LoanApplicationSubmittedCommandHandler(ILoanRepository<LoanApplication> repository, ILoanRepository<Operator> operatorrepository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _operatorrepository = operatorrepository;
        }

        protected override async Task<Guid> HandleAsync(LoanApplicationSubmittedCommand request)
        {
            Operator? x = _operatorrepository.WithLogin(request.operatorId).Result;
            Guard.Against.NotFound(x);
            var created = LoanApplication.Create(request.loanData, x);
            _repository.Insert(created);
            await UnitOfWork.CommitAsync();
            return created.Id;
        }
    }
}
