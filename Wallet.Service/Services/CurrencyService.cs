using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wallet.Database;
using Wallet.Database.Entities;
using Wallet.Models.Models;
using Wallet.Service.Cache;

namespace Wallet.Service.Services;

public interface ICurrencyService
{
    Task InsertOrUpdateCurrencyRates(CurrencyRateResponse response);
    Task<CurrencyResponse> GetCurrency(string currencyCode);
    Task PopulateCacheAsync();

    decimal ConvertAmount(
        decimal amount,
        string walletCurrencyCode,
        string toCurrencyCode,
        decimal currencyRate
    );
}

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyCacheService _currencyCacheService;
    private readonly WalletDbContext _walletDbContext;

    public CurrencyService(WalletDbContext walletDbContext, ICurrencyCacheService currencyCacheService)
    {
        _walletDbContext = walletDbContext;
        _currencyCacheService = currencyCacheService;
    }

    public async Task InsertOrUpdateCurrencyRates(CurrencyRateResponse response)
    {
        var newCurrencies = response.Rates
            .Where(kvp => !_walletDbContext.CurrencyCodes.Any(c => c.CurrencyCode == kvp.Currency))
            .Select(kvp => new CurrencyEntity { Id = Guid.NewGuid(), CurrencyCode = kvp.Currency })
            .ToList();

        if (newCurrencies.Count > 0)
        {
            _walletDbContext.CurrencyCodes.AddRange(newCurrencies);
            await _walletDbContext.SaveChangesAsync();
        }

        // Ensure EUR base currency is always present
        var eurCurrency = await _walletDbContext.CurrencyCodes
            .FirstOrDefaultAsync(c => c.CurrencyCode == "EUR");

        if (eurCurrency == null)
        {
            eurCurrency = new CurrencyEntity { Id = Guid.NewGuid(), CurrencyCode = "EUR" };
            _walletDbContext.CurrencyCodes.Add(eurCurrency);

            _walletDbContext.CurrencyRates.Add(new CurrencyRatesEntity
            {
                Id = Guid.NewGuid(),
                CurrencyCode = "EUR",
                Rate = 1.0m,
                ConversionDate = DateTime.UtcNow
            });

            await _walletDbContext.SaveChangesAsync();
        }

        Console.WriteLine("Ensured EUR base currency exists.");

        var values = string.Join(", ",
            response.Rates.Select(r =>
                $"(NEWID(), '{r.Currency}', {r.Rate.ToString(CultureInfo.InvariantCulture)}, '{response.Date:yyyy-MM-dd HH:mm:ss}')"));

        var sql = $"""
                       MERGE INTO CurrencyRates AS Target
                       USING (VALUES {values}) AS Source (Id, CurrencyCode, Rate, ConversionDate)
                       ON Target.CurrencyCode = Source.CurrencyCode AND Target.ConversionDate = Source.ConversionDate
                       WHEN MATCHED THEN
                           UPDATE SET Rate = Source.Rate, ConversionDate = Source.ConversionDate
                       WHEN NOT MATCHED THEN
                           INSERT (Id, CurrencyCode, Rate, ConversionDate)
                           VALUES (Source.Id, Source.CurrencyCode, Source.Rate, Source.ConversionDate);
                   """;

        await _walletDbContext.Database.ExecuteSqlRawAsync(sql);

        Console.WriteLine($"Successfully merged {response.Rates.Count} currency rates into database");
    }

    public async Task<CurrencyResponse> GetCurrency(string currencyCode)
    {
        var currencyEntity = _currencyCacheService.GetCurrencyRateCode(currencyCode);

        if (currencyEntity.CurrencyCode.IsNullOrEmpty())
        {
            await PopulateCacheAsync();
            currencyEntity = _currencyCacheService.GetCurrencyRateCode(currencyCode);
        }

        return currencyEntity == null
            ? throw new Exception($"Currency with code {currencyCode} not found")
            : new CurrencyResponse { Currency = currencyEntity.CurrencyCode, Rate = currencyEntity.Rate };
    }

    public async Task PopulateCacheAsync()
    {
        var currencyRates = await GetAllCurrencies();
        await _currencyCacheService.SetCurrencyRates(currencyRates);
        Console.WriteLine("Successfully Populated Cache");
    }

    public decimal ConvertAmount(
        decimal amount,
        string walletCurrencyCode,
        string toCurrencyCode,
        decimal currencyRate
    )
    {
        if (toCurrencyCode == walletCurrencyCode)
            return amount;
        return amount * currencyRate;
    }

    private async Task<Dictionary<string, decimal>> GetAllCurrencies()
    {
        return await _walletDbContext.CurrencyRates
            .GroupBy(cr => cr.CurrencyCode)
            .Select(g => g
                .OrderByDescending(cr => cr.ConversionDate)
                .FirstOrDefault())
            .ToDictionaryAsync(k => k!.CurrencyCode, v => v!.Rate);
    }
}