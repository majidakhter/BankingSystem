namespace BankingApp.AccountManagement.Application.Transfer.Models
{
    public class TransferDTO
    {
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? Balance { get; set; }
        public Guid AccountId { get; set; }
    }
}
