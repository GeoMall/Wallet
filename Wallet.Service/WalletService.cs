using Wallet.Database;
using Wallet.Service.Models;

namespace Wallet.Service;

public class WalletService
{
    private readonly WalletDbContext _walletDbContext;

    public WalletService(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<WalletResponse> CreateWallet(WalletRequest request)
    {
        var wallet = new WalletResponse
        {
            UserId = request.UserId,
            Currency = request.Currency
        };

        //TODO: PRODUCE CREATE WALLET KAFKA MESSAGE AFTER SAVE

        try
        {
            _walletDbContext.Wallets.Add(wallet);
            await _walletDbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured while creating new wallet for user {request.UserId}: {e.Message}");
            throw;
        }

        return wallet;
    }

    public async Task<WalletResponse> GetWallet(Guid id)
    {
        var wallet = await _walletDbContext.Wallets.FindAsync(id);

        return wallet ?? throw new Exception("Wallet not found");
    }

    public async Task<WalletResponse> Deposit(Guid id, decimal amount)
    {
        WalletResponse wallet;

        try
        {
            wallet = await GetWallet(id);
            wallet.Balance += amount;

            await _walletDbContext.SaveChangesAsync();

            //TODO: PRODUCE DEPOSIT KAFKA MESSAGE
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured while depositing funds for wallet with id {id}: {e.Message}");
            throw;
        }

        return wallet;
    }

    public async Task<WalletResponse> Withdraw(Guid id, decimal amount)
    {
        WalletResponse wallet;

        try
        {
            wallet = await GetWallet(id);

            if (amount > 0 && wallet.Balance >= amount)
                wallet.Balance -= amount;

            await _walletDbContext.SaveChangesAsync();

            //TODO: PRODUCE WITHDRAWAL KAFKA MESSAGE
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured while withdrawing funds for wallet with id {id}: {e.Message}");
            throw;
        }

        return wallet;
    }
}