using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDDD.UserManagement.Core.Users.Models
{
    public class UserProfileDTO
    {
       
        /// <summary>
        /// 
        /// </summary>
        public int AccountNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? FullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Gender { get; set; }
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
        public string SSNNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AccountType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AccountBalance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AccountStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte[] ProfileImage { get; set; }
        /// <summary>
        /// BranchId
        /// </summary>
        public Guid? BranchId { get; set; }
    }
}
