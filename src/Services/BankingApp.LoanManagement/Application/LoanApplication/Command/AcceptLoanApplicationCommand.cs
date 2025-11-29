using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingApp.LoanManagement.Infrastructure.Repositories;
using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;

namespace BankingApp.LoanManagement.Application.LoanApplicationCommands
{

    public sealed class AcceptLoanApplicationCommand : IUpdateCommand<CommandResult>
    {
        public Guid LoanApplicationId { get; private set; }
        public Guid OperatorId { get; private set; }
        public AcceptLoanApplicationCommand(Guid loanApplicationId, Guid operatorId)
        {
            LoanApplicationId = loanApplicationId;
            OperatorId = operatorId;
        }
    }
    public sealed class AcceptLoanApplicationCommandHandler : UpdateCommandHandler<AcceptLoanApplicationCommand, CommandResult>
    {
        private readonly ILoanRepository<LoanApplication> _repository;
        private readonly ILoanRepository<Operator> _operatorrepository;
        private readonly ILogger<AcceptLoanApplicationCommandHandler> _logger;
        public AcceptLoanApplicationCommandHandler(ILoanRepository<LoanApplication> repository, ILoanRepository<Operator> operatorrepository,
            ILogger<AcceptLoanApplicationCommandHandler> logger, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
            _operatorrepository = operatorrepository;
            _logger = logger;
        }

        public override async Task<CommandResult> Handle(AcceptLoanApplicationCommand request, CancellationToken cancellationToken)
        {
            var operato = await _operatorrepository.GetByIdAsync(request.OperatorId);
            var loanapplication = await _repository.GetByIdAsync(request.LoanApplicationId);
            if (loanapplication == null)
            {
                new ArgumentException("loanapplicationId does not exist");
            }
            loanapplication!.Accept(operato!, loanapplication.Customer.CustomerId);

            _repository.Update(loanapplication);
            await UnitOfWork.CommitAsync();
            return new CommandResult { IsSuccess = true };
        }
    }
}
