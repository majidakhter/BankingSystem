using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingAppDDD.CustomerManagement.Application.Customers.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerType : Enumeration
    {
        /// <summary>
        /// 
        /// </summary>
        public static CustomerType RegularCustomer = new CustomerType(1, nameof(RegularCustomer).ToLowerInvariant());
        /// <summary>
        /// 
        /// </summary>
        public static CustomerType CorporateCustomer = new CustomerType(2, nameof(CorporateCustomer).ToLowerInvariant());
        /// <summary>
        /// 
        /// </summary>
        public static CustomerType VisitorCustomer = new CustomerType(3, nameof(VisitorCustomer).ToLowerInvariant());
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public CustomerType(CustomerType t) : base(t.Id, t.Name)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public CustomerType()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public CustomerType(int id, string name)
         : base(id, name)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CustomerType> List() =>
            new[] { RegularCustomer, CorporateCustomer, VisitorCustomer};
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="AccountDomainException"></exception>
        public static CustomerType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for CustomerType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="AccountDomainException"></exception>
        public static CustomerType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for CustomerType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
