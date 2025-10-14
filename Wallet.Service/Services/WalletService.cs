using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Wallet.Database;
using Wallet.Database.Entities;
using Wallet.Models.Models;
using Wallet.Service.Factories;

namespace Wallet.Service.Services;

public interface IWalletService
{
    Task<WalletCreateResponse> CreateWallet(WalletRequest request);
    Task<WalletResponse> GetWallet(Guid id);

    Task<AdjustBalanceWalletResponse> AdjustBalance(
        Guid id,
        decimal amount,
        string currencyCode,
        string strategy
    );
}

public class WalletService : IWalletService
{
    private readonly CurrencyService _currencyService;
    private readonly StrategyFactory _strategyFactory;
    private readonly WalletDbContext _walletDbContext;

    public WalletService(
        WalletDbContext walletDbContext,
        CurrencyService currencyService,
        StrategyFactory strategyFactory
    )
    {
        _walletDbContext = walletDbContext;
        _currencyService = currencyService;
        _strategyFactory = strategyFactory;
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

    public async Task<AdjustBalanceWalletResponse> AdjustBalance(
        Guid id,
        decimal amount,
        string currencyCode,
        string strategy
    )
    {
        WalletResponse wallet;
        try
        {
            wallet = await GetWallet(id);

            var currency = await _currencyService.GetCurrency(currencyCode);
            var convertedAmount =
                _currencyService.ConvertAmount(amount, wallet.Currency, currency.Currency, currency.Rate);

            wallet.Balance = _strategyFactory.Adjust(convertedAmount, wallet.Balance, strategy);

            //Safety to concurrent API calls with the same wallet ID
            await _walletDbContext.Database.ExecuteSqlRawAsync(
                "UPDATE Wallets SET Balance = @p0 WHERE Id =  @p1",
                new SqlParameter("@p0", wallet.Balance),
                new SqlParameter("@p1", id)
            );
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured while adjusting wallet balance for wallet with id {id}: {e.Message}");
            throw;
        }

        return new AdjustBalanceWalletResponse { Id = wallet.Id, Balance = wallet.Balance };
    }
}