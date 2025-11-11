using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDDD.CustomerManagement.Core.Customers.DomainEvents
{
    /// <summary>
    /// 
    /// </summary>
    public record class CustomerRegistered : DomainEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CustomerId { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNo { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int CustType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public BirthDate DateOfBirth { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public AddressData PermanentAddress { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="phoneNo"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="custType"></param>
        /// <param name="permanentAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        //public AddressData PresentAddress { get; private set; }

        public static CustomerRegistered Create(
            Guid customerId,
            string name,
            string email,
            string phoneNo,
            DateOnly dateOfBirth,
            int custType,
            AddressData permanentAddress)
            //AddressData presentAddress)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(customerId));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            return new CustomerRegistered(
                customerId,
                name,
                email,
                phoneNo,
                dateOfBirth,
                custType,
                permanentAddress);
        }

        private CustomerRegistered(
            Guid customerId,
            string name,
            string email,
            string phoneNo,
            DateOnly dateOfBirth,
            int custType,
            AddressData permanentAddress)
        {
            CustomerId = customerId;
            Name = name;
            Email = email;
            PhoneNo = phoneNo;
            DateOfBirth = BirthDate.Create(dateOfBirth);
            CustType = custType;
            PermanentAddress = permanentAddress;
        }
    }
}
