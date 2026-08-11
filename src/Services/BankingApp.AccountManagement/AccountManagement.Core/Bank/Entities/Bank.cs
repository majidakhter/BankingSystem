using BankingAppDDD.Domains.Abstractions.Entities;

namespace BankingAppDDD.Domains.Banks.Entities
{
    public sealed class Bank : EntityBase, IAccountNonGenericRepo
    {
        public string Name { get; private set; }
        public DateTime? DateAdded { get; private set; }
        private Bank(string name, DateTime dateAdded)
        {
            Name = name;
            DateAdded = dateAdded;
        }
        public static Bank Create(string name, DateTime dateadded)
        {
            var bank = new Bank(name, dateadded);
            return bank;
        }
        private Bank()
        {

        }
    }
}
