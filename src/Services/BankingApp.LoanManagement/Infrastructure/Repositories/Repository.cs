
using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingAppDDD.Domains.Abstractions.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankingApp.LoanManagement.Infrastructure.Repositories
{
    public class Repository<T> : ILoanRepository<T> where T : EntityBase, ILoanNonGenericRepository
    {
        private readonly CreditMgmtDbContext _context;
        private readonly DbSet<T> _entitySet;
       // private readonly IAccountNonGenericRepo _repo;
        public Repository(CreditMgmtDbContext context)
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

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<T>().FirstOrDefaultAsync() : _context.Set<T>().FirstOrDefaultAsync(predicate);
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

        public async Task<T?> WithLogin(Guid id)
        {
            return id == Guid.Empty ? await _context.Set<T>()!.FirstOrDefaultAsync() : await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public  IQueryable<LoanApplication> GetLoanApplicationBySearchParam(string applicationNumber, Guid customerNationalIdentifier, Guid decisionBy, Guid registeredBy)
        {
            var result =  _context.LoanApplications.Where(d => d.Number == applicationNumber && d.Customer.CustomerId == customerNationalIdentifier && d.Decision.DecisionBy == decisionBy && d.Registration.RegisteredBy == registeredBy);
            return result;           ;
            
        }
    }
}