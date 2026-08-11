using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.Domains.Users.DomainEvents
{
    public record class UserRegisteredDomainEvent : DomainEvent
    {
        public Guid UserId { get; private set; }
        public Guid KeyCloakUserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNo { get; private set; }
        public int UserType { get; private set; }
        public BirthDate DateOfBirth { get; private set; }
        public string Gender { get; private set; }
        public string SSN { get; private set; }
        public decimal OpeningAmount { get; private set; }
        public int AccountTypeId { get; private set; }
        public byte[] ProfilePicture { get; private set; }
        public AddressData PermanentAddress { get; private set; }
        public Guid? BranchId { get; private set; }

        public static UserRegisteredDomainEvent Create(
            Guid userId,
            Guid keycloakUserId,
            string firstname,
            string lastname,
            string email,
            string phoneNo,
            DateOnly dateOfBirth,
            int userType,
            string gender,
            string ssn,
            byte[] profilePicture,
            AddressData permanentAddress,
            Guid? branchId = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(firstname))
                throw new ArgumentNullException(nameof(firstname));
            if (string.IsNullOrEmpty(lastname))
                throw new ArgumentNullException(nameof(lastname));
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            return new UserRegisteredDomainEvent(
                userId,
                keycloakUserId,
                firstname,
                lastname,
                email,
                phoneNo,
                dateOfBirth,
                userType,
                gender,
                ssn,
                profilePicture,
                permanentAddress,
                branchId);
        }

        private UserRegisteredDomainEvent(
            Guid userId,
            Guid keyCloakUserId,
            string firstname,
            string lastname,
            string email,
            string phoneNo,
            DateOnly dateOfBirth,
            int userType,
            string gender,
            string ssn,
            byte[] profilePicture,
            AddressData permanentAddress,
            Guid? branchId = null)
        {
            UserId = userId;
            KeyCloakUserId = keyCloakUserId;
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            PhoneNo = phoneNo;
            DateOfBirth = BirthDate.Create(dateOfBirth);
            UserType = userType;
            Gender = gender;
            SSN = ssn;
            ProfilePicture = profilePicture;
            PermanentAddress = permanentAddress;
            BranchId = branchId;
        }
    }
}
