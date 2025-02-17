using DigitalWallet.Core.Models;

namespace DigitalWallet.Core.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet> GetByUserIdAsync(int userId);
        Task<Wallet> CreateAsync(Wallet wallet);
        Task UpdateBalanceAsync(int walletId, decimal newBalance);
    }
}