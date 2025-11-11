using MediatR;

namespace BankingAppDDD.Applications.Abstractions.Queries
{
    public abstract record Query<T> : IRequest<T>;
}
