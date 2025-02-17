using DigitalWallet.Core.Models;

namespace DigitalWallet.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
        Task UpdateStatusAsync(int transactionId, string status);
    }
}