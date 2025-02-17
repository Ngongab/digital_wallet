namespace DigitalWallet.Core.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Wallet FromWallet { get; set; }
        public Wallet ToWallet { get; set; }
    }
}