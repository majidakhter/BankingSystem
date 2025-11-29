using BankingAppDDD.CustomerManagement.Application.Customers.Models;
using BankingAppDDD.CustomerManagement.Core.Customers.DomainEvents;
using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.CustomerManagement.Core.Customers.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Customer : EntityBase, IAggregateRoot
    {
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
        public string Email { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public BirthDate DateOfBirth { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public CustomerType CustomerType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int CustomerTypeId { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime DateAdded { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdatedOn { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Address PermanentAddress { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public LoanApplicationStatus LoanStatus { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerData"></param>
        /// <param name="addressData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BusinessRuleException"></exception>
        public static Customer Create(CustomerData customerData, AddressData addressData)
        {
            var (Email, Name, PhoneNo, DateOfBirth, CustType) = customerData ?? throw new ArgumentNullException(nameof(customerData));

            if (string.IsNullOrWhiteSpace(Email))
                throw new BusinessRuleException("Customer email cannot be null or whitespace.");

            if (string.IsNullOrWhiteSpace(Name))
                throw new BusinessRuleException("Customer name cannot be null or whitespace.");

            if (string.IsNullOrWhiteSpace(PhoneNo))
                throw new BusinessRuleException("Customer Phone No cannot be null or whitespace.");

            return new Customer(customerData, addressData);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCustomerData"></param>
        /// <param name="newAddressData"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BusinessRuleException"></exception>
        public void UpdateInformation(CustomerUpdateData newCustomerData, AddressData newAddressData)
        {
            var (CustomerId, Email, Name, PhoneNo, DateOfBirth, CustType) = newCustomerData ?? throw new ArgumentNullException(nameof(newCustomerData));
            if (string.IsNullOrWhiteSpace(Name))
                throw new BusinessRuleException("Customer name cannot be null or whitespace.");
            if (string.IsNullOrWhiteSpace(Email))
                throw new BusinessRuleException("Customer name cannot be null or whitespace.");

            if (string.IsNullOrWhiteSpace(PhoneNo))
                throw new BusinessRuleException("Customer Phone No cannot be null or whitespace.");

            var @event = CustomerUpdated.Create(
                CustomerId,
                Name,
                PhoneNo,
                Email,
                CustType,
                DateOfBirth,
                newAddressData
                );

            AddDomainEvent(@event);
            Apply(@event);
        }
        private void Apply(CustomerUpdated @event)
        {
            Id = @event.CustomerId;
            DateOfBirth = BirthDate.Create(@event.DateOfBirth.Value);
            Name = @event.Name;
            PermanentAddress = Address.Create(@event.PermanentAddress);
            PhoneNo = @event.PhoneNo;
            CustomerTypeId = @event.CustType;
            Email = @event.Email;
            UpdatedOn = @event.Timestamp;
        }
        private void Apply(CustomerRegistered @event)
        {
            Id = @event.CustomerId;
            Email = @event.Email;
            Name = @event.Name;
            PhoneNo = @event.PhoneNo;
            DateOfBirth = BirthDate.Create(@event.DateOfBirth.Value);
            CustomerTypeId = @event.CustType;
            PermanentAddress = Address.Create(@event.PermanentAddress);
            DateAdded = @event.Timestamp;
        }
        private Customer(CustomerData customerData, AddressData addressData)
        {
            var CustomerTypeIdEnumEnums = CustomerType.List().First(x => x.Id == customerData.customerType).Id;
            var @event = CustomerRegistered.Create(
                Guid.NewGuid(),
                customerData.Name,
                customerData.Email,
                customerData.phoneno,
                customerData.dateOfBirth,
                CustomerTypeIdEnumEnums,
                addressData);

            AddDomainEvent(@event);
            Apply(@event);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loanApplicationStatus"></param>
        public void UpdateLoanApplicationStatus(LoanApplicationStatus loanApplicationStatus)
        {
            LoanStatus = loanApplicationStatus;
        }
        private Customer()
        {

        }

    }
}
