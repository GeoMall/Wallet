using Wallet.Database;
using Wallet.Database.Entities;
using Wallet.Models.Models;

namespace Wallet.Service;

public class WalletService
{
    private readonly WalletDbContext _walletDbContext;

    public WalletService(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<WalletCreateResponse> CreateWallet(WalletRequest request)
    {
        var wallet = new WalletEntity
        {
            Id = Guid.NewGuid(),
            CurrencyCode = request.Currency,
            Balance = 0m
        };

        try
        {
            _walletDbContext.Wallets.Add(wallet);
            await _walletDbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured while creating new wallet: {e.Message}");
            throw;
        }

        return new WalletCreateResponse { Id = wallet.Id };
    }

    public async Task<WalletResponse> GetWallet(Guid id)
    {
        var wallet = await _walletDbContext.Wallets.FindAsync(id);

        return wallet == null
            ? throw new Exception("Wallet not found")
            : new WalletResponse { Id = wallet!.Id, Balance = wallet.Balance, Currency = wallet.CurrencyCode };
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