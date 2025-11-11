using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.LoanManagement.Application.LoanApplicationModels
{
    public class CreditCardsDTO
    {
      
        public Guid CardId { get; set; }
        public Guid CustomerId { get; set; } // change this to cardholdername
        public string? CardNumber { get; set; }
        public CardType CardType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int CVV { get; set; }
        public double CreditLimit { get; set; }
        public CardStatus Status { get; set; }
        public Guid AccountId { get; set; }
        //change the status to active
        public void Activate()
        {
            throw new NotImplementedException();
        }
        //change the status to block
        public void Block()
        {
            throw new NotImplementedException();
        }
        //set the status to expired
        public void Expire()
        {
            throw new NotImplementedException();
        }
        //update the card's pin here newpin number is input parameter
        public void ChangePIN(string newPin)
        {
            throw new NotImplementedException();
        }
        //check if the card caan process given transaction considering its status and balance . transaction is input param here
        public void CanProcess()
        {
            throw new NotImplementedException();
        }

    }
}
