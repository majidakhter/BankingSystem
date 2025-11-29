using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Domains.Abstractions.Entities;

namespace BankingApp.AccountManagement.Core.Customers.Entities
{
    public sealed class Customer : EntityBase, IAccountNonGenericRepo
    {

        private Customer(Guid customerId)
        {
            this.CustomerId = customerId;
        }

        public int NumberOfAccounts { get; private set; }
        public Guid CustomerId { get; private set; }
        public static Customer Create(Guid customerId)
        {
            return new Customer(customerId);
        }

        public void SetOneAccountClosed()
        {
            NumberOfAccounts = NumberOfAccounts - 1;
        }

        public void SetOneAccountAdded()
        {
            NumberOfAccounts = NumberOfAccounts + 1;
        }
    }
}
