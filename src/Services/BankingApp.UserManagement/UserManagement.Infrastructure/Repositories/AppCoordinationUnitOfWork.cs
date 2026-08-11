using BankingAppDDD.Applications.Abstractions.Repositories;
using System.Transactions;

namespace BankingAppDDD.UserManagement.Infrastructure.Repositories
{
    public class AppCoordinationUnitOfWork: IAppCoordinationUnitOfWork
    {
        public IUserUnitOfWork UserUow { get; }
        public IAccountUnitOfWork AccountUow { get; }

        public AppCoordinationUnitOfWork(IUserUnitOfWork userUow, IAccountUnitOfWork accountUow)
        {
            UserUow = userUow;
            AccountUow = accountUow;
        }

        public async Task<bool> CommitAllAsync(CancellationToken cancellationToken = default)
        {
            // Use TransactionScope for multi-dbcontext atomicity in EF Core

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool result = await UserUow.CommitAsync() && await AccountUow.CommitAsync();

                if (result)
                {
                    scope.Complete();
                }

                return result;
            }
           
        }
    }
}
