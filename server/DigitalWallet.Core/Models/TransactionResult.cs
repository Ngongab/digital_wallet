namespace DigitalWallet.Core.Models
{
    public class TransactionResult
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal NewBalance { get; set; }
    }
}