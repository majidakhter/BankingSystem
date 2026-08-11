using BankingAppDDD.Domains.Abstractions.Entities;
using BankingAppDDD.Domains.Abstractions.Guards;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;
using BankingAppDDD.Domains.Users.DomainEvents;
using BankingAppDDD.Domains.Users.Models;
using Microsoft.AspNetCore.Http;

namespace BankingAppDDD.UserManagement.Core.Users.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class User : EntityBase, IAggregateRoot
    {
        /// <summary>
        /// KeyCloakUserId
        /// </summary>
        public Guid KeyCloakUserId { get; private set; }
        /// <summary>
        /// FirstName
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// LastName
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// PhoneNo
        /// </summary>
        public string PhoneNo { get; private set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// DateOfBirth
        /// </summary>
        public BirthDate DateOfBirth { get; private set; }
        /// <summary>
        /// CustomerType
        /// </summary>
        public UserType UserType { get; private set; }
        /// <summary>
        /// CustomerTypeId
        /// </summary>
        public int UserTypeId { get; private set; }
        /// <summary>
        /// Gender
        /// </summary>
        public string Gender { get; private set; }
        /// <summary>
        /// SSN
        /// </summary>
        public string SSN { get; private set; }
        /// <summary>
        /// ProfileImage
        /// </summary>
        public byte[] ProfileImage { get; private set; }
        /// <summary>
        /// DateAdded
        /// </summary>
        public DateTime DateAdded { get; private set; }
        /// <summary>
        /// UpdatedOn
        /// </summary>
        public DateTime UpdatedOn { get; private set; }
        /// <summary>
        /// PermanentAddress
        /// </summary>
        public Address PermanentAddress { get; private set; }
        /// <summary>
        /// LoanStatus
        /// </summary>
        public LoanApplicationStatus LoanStatus { get; private set; }
        /// <summary>
        /// BranchId
        /// </summary>
        public Guid? BranchId { get; private set; }
        /// <summary>
        /// Create Customer
        /// </summary>
        /// <param name="customerData"></param>
        /// <param name="addressData"></param>
        /// <param name="keyCloakUserId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BusinessRuleException"></exception>
        public static User Create(UserData customerData, AddressData addressData, Guid keyCloakUserId, Guid? branchId = null)
        {
            var (UserName, Password,Email, FirstName, LastName, PhoneNo, DateOfBirth, UserType, Gender, SSNumber, ProfileImage) = customerData ?? throw new ArgumentNullException(nameof(customerData));

            if (string.IsNullOrWhiteSpace(Email))
                throw new BusinessRuleException("User email cannot be null or whitespace.");

            if (string.IsNullOrWhiteSpace(FirstName))
                throw new BusinessRuleException("User FirstName cannot be null or whitespace.");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new BusinessRuleException("User FirstName cannot be null or whitespace.");

            if (string.IsNullOrWhiteSpace(PhoneNo))
                throw new BusinessRuleException("User Phone No cannot be null or whitespace.");

            return new User(customerData, addressData, keyCloakUserId, branchId);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCustomerData"></param>
        /// <param name="newAddressData"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="BusinessRuleException"></exception>
        public void UpdateInformation(UserUpdateData newCustomerData, AddressData newAddressData)
        {
            var (CustomerId, Email, FirstName, LastName, PhoneNo, DateOfBirth, CustType, Gender, SSN, ProfileImage) = newCustomerData ?? throw new ArgumentNullException(nameof(newCustomerData));
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new BusinessRuleException("FirstName  cannot be null or whitespace.");
            if (string.IsNullOrWhiteSpace(LastName))
                throw new BusinessRuleException("LastName cannot be null or whitespace.");
            if (string.IsNullOrWhiteSpace(Email))
                throw new BusinessRuleException("User Email cannot be null or whitespace.");

            if (string.IsNullOrWhiteSpace(PhoneNo))
                throw new BusinessRuleException("User Phone No cannot be null or whitespace.");
            var ProfilePic = ConvertToByteArray(ProfileImage);
            var @event = UserUpdatedDomainEvent.Create(
                CustomerId,
                FirstName,
                LastName,
                PhoneNo,
                Email,
                CustType,
                DateOfBirth,
                Gender,
                SSN,
                ProfilePic,
                newAddressData
                );

            AddDomainEvent(@event);
            Apply(@event);
        }
        private void Apply(UserUpdatedDomainEvent @event)
        {
            Id = @event.UserId;
            DateOfBirth = BirthDate.Create(@event.DateOfBirth.Value);
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            PermanentAddress = Address.Create(@event.PermanentAddress);
            PhoneNo = @event.PhoneNo;
            UserTypeId = @event.UserType;
            Email = @event.Email;
            Gender = @event.Gender;
            SSN = @event.SSN;
            ProfileImage = @event.ProfileImage;
            UpdatedOn = @event.Timestamp;
        }
        private void Apply(UserRegisteredDomainEvent @event)
        {
            Id = @event.UserId;
            KeyCloakUserId = @event.KeyCloakUserId;
            Email = @event.Email;
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            PhoneNo = @event.PhoneNo;
            DateOfBirth = BirthDate.Create(@event.DateOfBirth.Value);
            UserTypeId = @event.UserType;
            Gender = @event.Gender;
            SSN = @event.SSN;
            ProfileImage = @event.ProfilePicture;
            PermanentAddress = Address.Create(@event.PermanentAddress);
            DateAdded = @event.Timestamp;
            BranchId = @event.BranchId;
        }
        private User(UserData customerData, AddressData addressData, Guid keyCloakUserId, Guid? branchId = null)
        {
            var CustomerTypeIdEnumEnums = UserType.List().First(x => x.Id == customerData.UserType).Id;
            var ProfilePic = (customerData.ProfileImageBytes != null && customerData.ProfileImageBytes.Length > 0)
                ? customerData.ProfileImageBytes
                : ConvertToByteArray(customerData.ProfileImage);
            var @event = UserRegisteredDomainEvent.Create(
                Guid.NewGuid(),
                keyCloakUserId,
                customerData.FirstName,
                customerData.LastName,
                customerData.Email,
                customerData.PhoneNo,
                customerData.DateOfBirth,
                CustomerTypeIdEnumEnums,
                customerData.Gender,
                customerData.SSNumber,
                ProfilePic,
                addressData,
                branchId);

            AddDomainEvent(@event);
            Apply(@event);
        }


        private byte[] ConvertToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Array.Empty<byte>();
            }

            using var stream = file.OpenReadStream();
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loanApplicationStatus"></param>
        public void UpdateLoanApplicationStatus(LoanApplicationStatus loanApplicationStatus)
        {
            LoanStatus = loanApplicationStatus;
        }
        private User()
        {

        }

    }
}
