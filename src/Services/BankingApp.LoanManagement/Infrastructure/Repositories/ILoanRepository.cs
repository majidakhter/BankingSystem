using BankingApp.LoanManagement.Core.LoanApplicationsEntities;
using BankingAppDDD.Domains.Abstractions.Entities;
using System.Linq.Expressions;

namespace BankingApp.LoanManagement.Infrastructure.Repositories
{
    public interface ILoanRepository<T> where T : EntityBase, ILoanNonGenericRepository
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
        Task<T?> WithLogin(Guid id);
        IQueryable<LoanApplication> GetLoanApplicationBySearchParam(string applicationNumber, Guid customerNationalIdentifier, Guid decisionBy, Guid registeredBy);
    }
}
