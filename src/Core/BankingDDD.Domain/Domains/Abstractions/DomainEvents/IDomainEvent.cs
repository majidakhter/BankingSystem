using MediatR;

namespace BankingAppDDD.Domains.Abstractions.DomainEvents
{
    public abstract record IDomainEvent : INotification;


}
