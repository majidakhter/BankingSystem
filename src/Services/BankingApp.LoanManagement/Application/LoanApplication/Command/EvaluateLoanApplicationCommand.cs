using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingApp.LoanManagement.Infrastructure.Abstraction;
using BankingApp.LoanManagement.Infrastructure.Factory;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;

namespace BankingApp.LoanManagement.Application.LoanApplicationCommands
{
    // public sealed record EvaluateLoanApplicationCommand(Guid loanApplicationId) : IUpdateCommand;

    public sealed class EvaluateLoanApplicationCommand : IUpdateCommand<CommandResult>
    {
        public Guid LoanApplicationId { get; private set; }
        public EvaluateLoanApplicationCommand(Guid loanApplicationId)
        {
            LoanApplicationId = loanApplicationId;
        }
    }
    public sealed class EvaluateLoanApplicationCommandHandler : UpdateCommandHandler<EvaluateLoanApplicationCommand, CommandResult>
    {
        private readonly ILoanRepository<LoanApplication> _repository;
        private readonly ScoringRulesFactory scoringRulesFactory;
        public EvaluateLoanApplicationCommandHandler(ILoanRepository<LoanApplication> repository, IDebtorRegistry debtorRegistry, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            this.scoringRulesFactory = new ScoringRulesFactory(debtorRegistry);
        }

        public override async Task<CommandResult> Handle(EvaluateLoanApplicationCommand request, CancellationToken cancellationToken)
        {
            var loanapplication = await _repository.GetByIdAsync(request.LoanApplicationId);
            if (loanapplication == null)
            {
                new ArgumentException("loanapplicationId does not exist");
            }
            loanapplication!.Evaluate(scoringRulesFactory.DefaultSet);

            _repository.Update(loanapplication);
            await UnitOfWork.CommitAsync();
            return new CommandResult { IsSuccess = true };
        }

    }
}
