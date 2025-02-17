using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;
using DigitalWallet.Core.Common;
using DigitalWallet.Core.Interfaces;
using DigitalWallet.Core.Models;
using Transaction = DigitalWallet.Core.Models.Transaction;

namespace DigitalWallet.Core.Services
{
    public class WalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<WalletService> _logger;

        public WalletService(
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository,
            ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<TransactionResult>> TransferMoneyAsync(
            int fromUserId,
            int toUserId,
            decimal amount,
            string description)
        {
            try
            {
                var (sourceWallet, targetWallet) = await GetWalletsAsync(fromUserId, toUserId);
                if (sourceWallet == null || targetWallet == null)
                    return Result<TransactionResult>.Failure("Wallet not found");

                if (sourceWallet.Balance < amount)
                    return Result<TransactionResult>.Failure("Insufficient funds");

                var transaction = await ProcessTransactionAsync(sourceWallet, targetWallet, amount, description);
                return Result<TransactionResult>.Success(new TransactionResult
                {
                    TransactionId = transaction.TransactionId,
                    Amount = amount,
                    NewBalance = sourceWallet.Balance - amount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during money transfer. FromUserId: {FromUserId}, ToUserId: {ToUserId}, Amount: {Amount}",
                    fromUserId, toUserId, amount);
                return Result<TransactionResult>.Failure("Transaction failed");
            }
        }

        public async Task<Result<Wallet>> GetWalletAsync(int userId)
        {
            try
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId).ConfigureAwait(false);
                return wallet != null 
                    ? Result<Wallet>.Success(wallet) 
                    : Result<Wallet>.Failure("Wallet not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet for user {UserId}", userId);
                return Result<Wallet>.Failure("Failed to retrieve wallet");
            }
        }

        private async Task<(Wallet source, Wallet target)> GetWalletsAsync(int fromUserId, int toUserId)
        {
            var sourceWallet = await _walletRepository.GetByUserIdAsync(fromUserId).ConfigureAwait(false);
            var targetWallet = await _walletRepository.GetByUserIdAsync(toUserId).ConfigureAwait(false);
            return (sourceWallet, targetWallet);
        }

        private async Task<Transaction> ProcessTransactionAsync(Wallet sourceWallet, Wallet targetWallet, decimal amount, string description)
        {
            var transaction = new Transaction
            {
                FromWalletId = sourceWallet.WalletId,
                ToWalletId = targetWallet.WalletId,
                Amount = amount,
                Description = description,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _walletRepository.UpdateBalanceAsync(sourceWallet.WalletId, sourceWallet.Balance - amount).ConfigureAwait(false);
                await _walletRepository.UpdateBalanceAsync(targetWallet.WalletId, targetWallet.Balance + amount).ConfigureAwait(false);

                transaction = await _transactionRepository.CreateAsync(transaction).ConfigureAwait(false);
                await _transactionRepository.UpdateStatusAsync(transaction.TransactionId, "COMPLETED").ConfigureAwait(false);

                scope.Complete();
            }

            _logger.LogInformation("Transfer completed successfully. TransactionId: {TransactionId}, Amount: {Amount}",
                transaction.TransactionId, amount);

            return transaction;
        }
    }
}
