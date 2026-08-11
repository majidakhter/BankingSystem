using BankingAppDDD.Applications.Abstractions.Commands;
using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper;
using Identity.Core.IAM.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BankingAppDDD.Identity.Application.IAM.Commands
{
    public sealed record RegisterIdentityCommand : Command
    {
        public UserIdentityData userData { get; set; } = new();
        public AddressData addressData { get; set; } = new();
        public int accountTypeId { get; set; } = 1;
        public decimal amount { get; set; } = 0;
        public Guid? branchId { get; set; }

        public RegisterIdentityCommand() { }

        public RegisterIdentityCommand(UserIdentityData userData, AddressData addressData, int accountTypeId, decimal amount, Guid? branchId = null)
        {
            this.userData = userData;
            this.addressData = addressData;
            this.accountTypeId = accountTypeId;
            this.amount = amount;
            this.branchId = branchId;
        }

        public void Deconstruct(out UserIdentityData userData, out AddressData addressData, out int accountTypeId, out decimal amount)
        {
            userData = this.userData;
            addressData = this.addressData;
            accountTypeId = this.accountTypeId;
            amount = this.amount;
        }
    }

    public sealed class RegisterIdentityCommandHandler : CommandHandler<RegisterIdentityCommand>
    {
        private readonly IKeycloakService _keycloakAdmin;
        private readonly IMediator _mediator;
        private readonly ILogger<RegisterIdentityCommandHandler> logger;

        public RegisterIdentityCommandHandler(
            ILogger<RegisterIdentityCommandHandler> _logger,
            IKeycloakService keycloakAdmin,
            IMediator mediator,
            IUnitOfWork? unitOfWork = null) : base(unitOfWork)
        {
            _keycloakAdmin = keycloakAdmin;
            _mediator = mediator;
            logger = _logger;
        }

        protected override async Task HandleAsync(RegisterIdentityCommand request)
        {
            request.userData?.EnsureProfileImageBytes();
            var (UserName, Password, Email, FirstName, LastName, PhoneNo, DateOfBirth, UserType, Gender, SSNumber, ProfileImage) = request.userData;
            var newUser = new UserCreationRequest(UserName, Email, FirstName, LastName, Password);

            Guid keyCloakUserId = Guid.Empty;
            if (_keycloakAdmin != null)
            {
                try
                {
                    keyCloakUserId = await _keycloakAdmin.CreateUserAsync(newUser);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to create user in Keycloak, falling back to generated GUID for local execution.");
                    keyCloakUserId = Guid.NewGuid();
                }
            }

            var account = RegisterAccount.Create(request.userData, request.addressData, keyCloakUserId, request.accountTypeId, request.amount, request.branchId);

            if (account.DomainEvents != null)
            {
                foreach (var domainEvent in account.DomainEvents)
                {
                    await _mediator.Publish(domainEvent);
                }
                account.ClearDomainEvents();
            }
        }
    }
}
