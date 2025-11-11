using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingAppDDD.Domains.Abstractions.Entities;
using System.Linq.Expressions;

namespace BankingApp.AccountManagement.Infrastructure.Repositories
{
    public interface IAccountRepository<T> where T : EntityBase, IAccountNonGenericRepo
    {
        IQueryable<T> GetAll(bool noTracking = true);
        Task<T?> GetByIdAsync(Guid id);
        void Insert(T entity);
        void Insert(List<T> entities);
        void Delete(T entity);
        void Remove(IEnumerable<T> entitiesToRemove);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null);
        void Update(T item);
        IQueryable<T> FetchMulti(Expression<Func<T, bool>> predicate = null);
        Task<Account> GetEntityById(Guid id);
    }
}
