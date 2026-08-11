using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;

namespace BankingAppDDD.Domains.Users.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserType : Enumeration
    {
        /// <summary>
        /// 
        /// </summary>
        public static UserType RegularCustomer = new UserType(1, nameof(RegularCustomer).ToLowerInvariant());
        /// <summary>
        /// 
        /// </summary>
        public static UserType CorporateCustomer = new UserType(2, nameof(CorporateCustomer).ToLowerInvariant());
        /// <summary>
        /// 
        /// </summary>
        public static UserType VisitorCustomer = new UserType(3, nameof(VisitorCustomer).ToLowerInvariant());
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public UserType(UserType t) : base(t.Id, t.Name)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public UserType()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public UserType(int id, string name)
         : base(id, name)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UserType> List() =>
            new[] { RegularCustomer, CorporateCustomer, VisitorCustomer };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="AccountDomainException"></exception>
        public static UserType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for UserType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="AccountDomainException"></exception>
        public static UserType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for UserType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
