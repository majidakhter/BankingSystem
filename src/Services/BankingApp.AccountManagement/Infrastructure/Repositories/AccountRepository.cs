
using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingAppDDD.Domains.Abstractions.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace BankingApp.AccountManagement.Infrastructure.Repositories
{
    public class AccountRepository<T> : IAccountRepository<T> where T : EntityBase, IAccountNonGenericRepo
    {
        private readonly AccountDbContext _context;
        private readonly DbSet<T> _entitySet;
       // private readonly IAccountNonGenericRepo _repo;
        public AccountRepository(AccountDbContext context)
        {
            _context = context;
            _entitySet = _context.Set<T>();
        }

        public IQueryable<T> GetAll(bool noTracking = true)
        {
            var set = _entitySet;
            if (noTracking)
            {
                return set.AsNoTracking();
            }
            return set;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _entitySet.FindAsync(id);
        }

        public void Insert(T entity)
        {
            _entitySet.Add(entity);
        }

        public void Insert(List<T> entities)
        {
            _entitySet.AddRange(entities);
        }

        public void Delete(T entity)
        {
            _entitySet.Remove(entity);
        }

        public void Remove(IEnumerable<T> entitiesToRemove)
        {
            _entitySet.RemoveRange(entitiesToRemove);
        }

        public  Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? _entitySet.FirstOrDefaultAsync() : _entitySet.FirstOrDefaultAsync(predicate);
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _context.Entry(item).State = EntityState.Modified;
        }
        public virtual IQueryable<T> FetchMulti(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<T>() : _context.Set<T>().Where(predicate);
        }
        public virtual async Task<int> BulkUpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updatePredicate)
        {
            return await _context.Set<T>().Where(predicate).UpdateAsync(updatePredicate);
        }
        public async Task<Account> GetEntityById(Guid id)
        {
            var entity = await _context.Set<Account>().FindAsync(id);
            if (entity != null)
            {
                await _context.Entry(entity)
                    .Collection(q => q.Credits).LoadAsync();

                await _context.Entry(entity)
                .Collection(q => q.Debits).LoadAsync();

                await _context.Entry(entity)
                    .Reference(q => q.AccountStatus).LoadAsync();
            }

            return entity;
        }

    }
}