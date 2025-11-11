using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.AccountManagement.Core.Banks.Entities
{
    public sealed class Bank : EntityBase, IAccountNonGenericRepo
    {
        public string Name { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Address BankAddress { get; private set; }
        public DateTime? DateAdded { get; private set; }
        private Bank(string name, string phoneNumber, AddressData bankAddress, DateTime dateAdded)
        {
            Name = name;
            PhoneNumber = PhoneNumber.Create(phoneNumber);
            BankAddress = Address.Create(bankAddress);
            DateAdded = dateAdded;
        }
        public static Bank Create(string name, string phoneNumber, AddressData bankAddress, DateTime dateadded)
        {
            var bank = new Bank(name, phoneNumber, bankAddress, dateadded);
            return bank;
        }
        private Bank()
        {

        }
    }
}
