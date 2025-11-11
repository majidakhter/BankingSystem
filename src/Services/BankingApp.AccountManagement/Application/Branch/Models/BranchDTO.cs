using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.AccountManagement.Application.Branches.Model
{
    public class BranchDTO
    {
        public Guid BranchId { get; set; }
        public string? Name { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public DateTime? DateAdded { get; set; }
        public string PhoneNumber { get; set; }
        public Guid BankId { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }


    }
}
