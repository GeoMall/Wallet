using Microsoft.EntityFrameworkCore;
using Quartz;
using Wallet.Client.ECB;
using Wallet.Database;

namespace Wallet.Scheduler.Jobs;

public class CurrencyUpdaterJob : IJob
{
    private readonly WalletDbContext _db;
    private readonly WalletsEcbClient _walletsEcbClient;

    public CurrencyUpdaterJob(
        WalletsEcbClient walletsEcbClient,
        WalletDbContext db
    )
    {
        _walletsEcbClient = walletsEcbClient;
        _db = db;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var result = await _walletsEcbClient.GetCurrencyRates();
            Console.WriteLine($"Fetched {result.Rates.Count} ECB rates for {result.Date}");

            //TODO: MOVE LOGIC IN A CURRENCY CLASS IN WALLET.DATABASE PROJ
            foreach (var rate in result.Rates)
            {
                var sql = @"
                    MERGE INTO CurrencyRates AS Target
                    USING (SELECT {0} AS CurrencyCode, {1} AS Rate, {2} AS ConversionDate) AS Source
                    ON Target.CurrencyCode = Source.CurrencyCode
                    WHEN MATCHED THEN 
                        UPDATE SET Rate = Source.Rate, ConversionDate = Source.ConversionDate
                    WHEN NOT MATCHED THEN
                        INSERT (CurrencyCode, Rate, ConversionDate)
                        VALUES (Source.CurrencyCode, Source.Rate, Source.ConversionDate);";

                await _db.Database.ExecuteSqlRawAsync(sql, rate.Currency, rate.Rate, result.Date);
            }

            Console.WriteLine($"Successfully merged {result.Rates.Count} currency rates into database");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An Error occurred while syncing ECB currency rates.");
        }
    }
}