
using BankingApp.AccountManagement.Infrastructure.Repositories;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.AccountManagement.Core.Branches.Entities
{
    public sealed class Branch : EntityBase, IAccountNonGenericRepo
    {
        public string Name { get; private set; }
        public string BranchCode { get; private set; }
        public DateTime? DateAdded { get; private set; }
        public Guid BankId { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public  Address BranchAddress { get;private set; }

        private Branch(string name, string branchCode, Guid bankId, string phoneNo, AddressData addressData)
         {
             this.Name = name;
             this.BranchCode = branchCode;
             this.BankId = bankId;
             this.BranchAddress = Address.Create(addressData);
             DateTime localDateTime = DateTime.Now;
            this.PhoneNumber = PhoneNumber.Create(phoneNo);
             this.DateAdded = localDateTime.ToUniversalTime();
         }
        public static Branch Create(string name, string branchCode, Guid bankId, string phoneNo, AddressData addressData)
        {
            var bank = new Branch(name, branchCode, bankId, phoneNo, addressData);
            return bank;
        }
        private Branch()
        {

        }
    }
}
