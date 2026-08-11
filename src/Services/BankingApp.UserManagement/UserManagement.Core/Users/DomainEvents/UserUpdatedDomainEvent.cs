using BankingAppDDD.Domains.Abstractions.DomainEvents;
using BankingAppDDD.Domains.Abstractions.Models;
using BankingAppDDD.Domains.Abstractions.ValueObjects;

namespace BankingAppDDD.Domains.Users.DomainEvents
{
    /// <summary>
    /// 
    /// </summary>
    public record class UserUpdatedDomainEvent : DomainEvent
    {
        /// <summary>
        /// CustomerId
        /// </summary>
        public Guid UserId { get; private set; }
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
        /// CustType
        /// </summary>
        public int UserType { get; private set; }
        /// <summary>
        /// PermanentAddress
        /// </summary>
        public AddressData PermanentAddress { get; private set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// DateOfBirth
        /// </summary>
        public BirthDate DateOfBirth { get; private set; }
        /// <summary>
        /// Gender
        /// </summary>
        public string Gender { get; private set; }
        /// <summary>
        /// SSN
        /// </summary>
        public string SSN { get; private set; }
        /// <summary>
        /// SSN
        /// </summary>
        public byte[] ProfileImage { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="phoneNo"></param>
        /// <param name="email"></param>
        /// <param name="userType"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="gender"></param>
        /// <param name="ssn"></param>
        /// <param name="permanentAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static UserUpdatedDomainEvent Create(
            Guid userId,
            string firstname,
            string lastname,
            string phoneNo,
            string email,
            int userType,
            DateOnly dateOfBirth,
            string gender,
            string ssn,
            byte[] profileImage,
            AddressData permanentAddress
            )
        {
            if (userId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(firstname))
                throw new ArgumentNullException(nameof(firstname));
            if (string.IsNullOrEmpty(lastname))
                throw new ArgumentNullException(nameof(lastname));
            if (string.IsNullOrEmpty(phoneNo))
                throw new ArgumentNullException(nameof(phoneNo));
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            return new UserUpdatedDomainEvent(
                userId,
                firstname,
                lastname,
                phoneNo,
                email,
                userType,
                dateOfBirth,
                gender,
                ssn,
                profileImage,
                permanentAddress);
        }

        private UserUpdatedDomainEvent(
            Guid userId,
            string firstname,
            string lastname,
            string phoneNo,
            string email,
            int userType,
            DateOnly dateOfBirth,
            string gender,
            string ssn,
            byte[] profileImage,
            AddressData permanentAddress
            )
        {
            UserId = userId;
            FirstName = firstname;
            LastName = lastname;
            PhoneNo = phoneNo;
            UserType = userType;
            DateOfBirth = BirthDate.Create(dateOfBirth);
            PermanentAddress = permanentAddress;
            Email = email;
            Gender = gender;
            SSN = ssn;
            ProfileImage = profileImage;
        }
    }
}
