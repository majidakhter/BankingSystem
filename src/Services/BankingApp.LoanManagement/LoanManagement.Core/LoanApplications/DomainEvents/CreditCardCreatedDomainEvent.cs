
using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.LoanApplications.Models;
using BankingAppDDD.Domains.LoanApplications.ValueObjects;

namespace BankingAppDDD.Domains.LoanApplications.DomainEvents
{
    public sealed record CreditCardCreatedDomainEvent(Guid Id, Guid customerId, string cardNumber, CardType cardType, DateTime expiryDate, int cVV, CreditLimit creditLimit) : DomainEvent;
}
