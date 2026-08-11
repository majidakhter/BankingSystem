using BankingAppDDD.Domains.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace BankingAppDDD.Domains.CustomerAccounts.Entities
{
    public sealed class UserAccount : EntityBase, IAccountNonGenericRepo
    {

        private UserAccount(Guid userId)
        {
            this.UserId = userId;
        }

        public int NumberOfAccounts { get; private set; }

        [Key]
        public Guid UserId { get; private set; }
        public static UserAccount Create(Guid userId)
        {
            return new UserAccount(userId);
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
