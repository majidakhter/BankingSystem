using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.CustomerManagement.Core.Customers.DomainEvents
{
    /// <summary>
    /// 
    /// </summary>
    public record class CustomerUpdated : DomainEvent
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
        public string PhoneNo { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int CustType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public AddressData PermanentAddress { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public BirthDate DateOfBirth { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="name"></param>
        /// <param name="phoneNo"></param>
        /// <param name="email"></param>
        /// <param name="custType"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="permanentAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static CustomerUpdated Create(
            Guid customerId,
            string name,
            string phoneNo,
            string email,
            int custType,
            DateOnly dateOfBirth,
            AddressData permanentAddress
            )
        {
            if (customerId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(customerId));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(phoneNo))
                throw new ArgumentNullException(nameof(phoneNo));
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            return new CustomerUpdated(
                customerId,
                name,
                phoneNo,
                email,
                custType,
                dateOfBirth,
                permanentAddress);
        }

        private CustomerUpdated(
            Guid customerId,
            string name,
            string phoneNo,
            string email,
            int custType,
            DateOnly dateOfBirth,
            AddressData permanentAddress
            )
        {
            CustomerId = customerId;
            Name = name;
            PhoneNo = phoneNo;
            CustType = custType;
            DateOfBirth = BirthDate.Create(dateOfBirth);
            PermanentAddress = permanentAddress;
            Email = email;
        }
    }
}
