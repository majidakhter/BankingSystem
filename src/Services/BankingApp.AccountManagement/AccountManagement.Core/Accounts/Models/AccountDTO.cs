
namespace BankingAppDDD.Domains.Accounts.Models
{
    public class AccountDTO
    {
        public Guid AccountId { get; set; }
        public Guid CustomerId { get;  set; }
        public int AccountNo { get;  set; }
        public int AccountTypeId { get;  set; }
        public int AccountStatusId { get;  set; }
        public DateTime? DateAdded { get;  set; }
        public DateTime? AccountUpdatedDate { get;  set; }
    }
}
