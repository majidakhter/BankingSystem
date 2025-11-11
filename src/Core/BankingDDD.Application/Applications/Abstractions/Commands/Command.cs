using MediatR;
namespace BankingAppDDD.Applications.Abstractions.Commands
{
    public abstract record Command : CommandBase<Unit>;

    public abstract record CommandBase<T> : IRequest<T>;
    public abstract record CreateCommand : IRequest<Guid>;
    public interface IUpdateCommand<out TResponse> : IRequest<TResponse> { }
}
