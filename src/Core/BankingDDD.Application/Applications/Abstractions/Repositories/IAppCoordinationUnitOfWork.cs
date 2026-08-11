namespace BankingAppDDD.Applications.Abstractions.Repositories
{
    public interface IAppCoordinationUnitOfWork
    {
        IUserUnitOfWork UserUow { get; }
        IAccountUnitOfWork AccountUow { get; }
        Task<bool> CommitAllAsync(CancellationToken cancellationToken = default);
    }
}
