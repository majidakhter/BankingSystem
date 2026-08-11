
namespace BankingAppDDD.Domains.Banks.Models
{
    public class BankDTO
    {
        public Guid BankId { get; set; }
        public string Name { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
