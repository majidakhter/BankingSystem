namespace BankingAppDDD.CustomerManagement.Application.Customers.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int LoanStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }
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
        public int CustomerTypeId { get; set; }
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
