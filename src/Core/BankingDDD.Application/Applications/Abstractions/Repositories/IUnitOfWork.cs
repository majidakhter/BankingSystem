
namespace BankingAppDDD.Applications.Abstractions.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> CommitAsync(CancellationToken cancellationToken = default);
    }
}
