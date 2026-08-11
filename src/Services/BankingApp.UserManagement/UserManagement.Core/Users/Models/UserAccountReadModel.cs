using BankingAppDDD.Domains.Abstractions.Entities;
using System;

namespace BankingAppDDD.UserManagement.Core.Users.Models
{
    /// <summary>
    /// CQRS Read Model representing User Account details in UserManagement bounded context
    /// Updated via asynchronous event choreography (e.g. MassTransit / MediatR integration events)
    /// </summary>
    public class UserAccountReadModel : EntityBase
    {
        public UserAccountReadModel() : base(Guid.NewGuid()) { }

        public UserAccountReadModel(Guid id, Guid userId, Guid keycloakUserId, int accountNumber, int accountType, int accountStatus, decimal accountBalance)
            : base(id)
        {
            UserId = userId;
            KeycloakUserId = keycloakUserId;
            AccountNumber = accountNumber;
            AccountType = accountType;
            AccountStatus = accountStatus;
            AccountBalance = accountBalance;
            LastUpdated = DateTime.UtcNow;
        }

        public Guid UserId { get; set; }
        public Guid KeycloakUserId { get; set; }
        public int AccountNumber { get; set; } = 1000004;
        public int AccountType { get; set; } = 1;
        public int AccountStatus { get; set; } = 1;
        public decimal AccountBalance { get; set; } = 500m;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
