using BankingAppDDD.Applications.Abstractions.Repositories;
using BankingAppDDD.Domains.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace BankingAppDDD.CustomerManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        private readonly CustomerDbContext _context;
        private readonly DbSet<T> _entitySet;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public Repository(CustomerDbContext context)
        {
            _context = context;
            _entitySet = _context.Set<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noTracking"></param>
        /// <returns></returns>
        public IQueryable<T> GetAll(bool noTracking = true)
        {
            var set = _entitySet;
            if (noTracking)
            {
                return set.AsNoTracking();
            }
            return set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _entitySet.FindAsync(id);
        }

        public void Insert(T entity)
        {
            _entitySet.Add(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void Insert(List<T> entities)
        {
            _entitySet.AddRange(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            _entitySet.Remove(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitiesToRemove"></param>
        public void Remove(IEnumerable<T> entitiesToRemove)
        {
            _entitySet.RemoveRange(entitiesToRemove);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<T>().FirstOrDefaultAsync() : _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _context.Entry(item).State = EntityState.Modified;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> FetchMulti(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<T>() : _context.Set<T>().Where(predicate);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updatePredicate"></param>
        /// <returns></returns>
        public virtual async Task<int> BulkUpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updatePredicate)
        {
            return await _context.Set<T>().Where(predicate).UpdateAsync(updatePredicate);
        }
    }
}