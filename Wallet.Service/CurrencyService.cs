using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Wallet.Database;
using Wallet.Models.Models;

namespace Wallet.Service;

public class CurrencyService
{
    private readonly WalletDbContext _walletDbContext;

    public CurrencyService(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task InsertOrUpdateCurrencyRates(CurrencyRateResponse response)
    {
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
}