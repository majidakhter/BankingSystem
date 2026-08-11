namespace BankingAppDDD.Domains.Users.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int LoanStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? FirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? LastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? PhoneNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateOnly DateOfBirth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime DateAdded { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AddressDTO? PermanentAddress { get; set; }
    }
}
