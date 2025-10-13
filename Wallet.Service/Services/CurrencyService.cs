using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Wallet.Database;
using Wallet.Models.Models;
using Wallet.Service.Cache;

namespace Wallet.Service.Services;

public class CurrencyService
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
        // Ensure EUR base currency is always present
        const string ensureEurSql =
            """
                IF NOT EXISTS (SELECT 1 FROM CurrencyRates WHERE CurrencyCode = 'EUR')
                BEGIN
                    INSERT INTO CurrencyRates (Id, CurrencyCode, Rate, ConversionDate)
                    VALUES (NEWID(), 'EUR', 1.0, GETDATE());
                END;
            """;

        await _walletDbContext.Database.ExecuteSqlRawAsync(ensureEurSql);
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
        var currencyEntity = await _currencyCacheService.GetCurrencyRateCode(currencyCode);

        return currencyEntity == null
            ? throw new Exception($"Currency with code {currencyCode} not found")
            : new CurrencyResponse { Currency = currencyEntity.CurrencyCode, Rate = currencyEntity.Rate };
    }

    public async Task<Dictionary<string, decimal>> GetAllCurrencies()
    {
        return await _walletDbContext.CurrencyRates.ToDictionaryAsync(k => k.CurrencyCode, v => v.Rate);
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
}