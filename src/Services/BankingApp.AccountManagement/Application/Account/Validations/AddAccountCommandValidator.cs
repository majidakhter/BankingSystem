using BankingApp.AccountManagement.Application.Accounts.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Application.Accounts.Validations
{
    public class AddAccountCommandValidator: AbstractValidator<AddAccountCommand>
    {
        public AddAccountCommandValidator(ILogger<AddAccountCommand> logger)
        {
            RuleFor(command => command.customerId).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
        
    }
}