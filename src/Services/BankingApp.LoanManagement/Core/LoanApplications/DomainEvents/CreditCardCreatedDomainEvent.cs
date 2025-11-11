using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Core.LoanApplications.ValueObjects;
using BankingAppDDD.Domains.Abstractions.DomainEvents;

namespace BankingApp.LoanManagement.Core.LoanApplicationsDomainEvents
{
    public sealed record CreditCardCreatedDomainEvent(Guid Id, Guid customerId, string cardNumber, CardType cardType, DateTime expiryDate, int cVV, CreditLimit creditLimit) : DomainEvent;
}
