using Quartz;
using Wallet.Client.ECB;
using Wallet.Service.Cache;
using Wallet.Service.Services;

namespace Wallet.Scheduler.Jobs;

public class CurrencyUpdaterJob : IJob
{
    private readonly ICurrencyCacheService _currencyCacheService;
    private readonly CurrencyService _currencyService;
    private readonly WalletsEcbClient _walletsEcbClient;

    public CurrencyUpdaterJob(
        WalletsEcbClient walletsEcbClient,
        CurrencyService currencyService,
        ICurrencyCacheService currencyCacheService
    )
    {
        _walletsEcbClient = walletsEcbClient;
        _currencyService = currencyService;
        _currencyCacheService = currencyCacheService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var response = await _walletsEcbClient.GetCurrencyRates();
            Console.WriteLine($"Fetched {response.Rates.Count} ECB rates for time period {response.Date}");

            await _currencyService.InsertOrUpdateCurrencyRates(response);
            await _currencyCacheService.SetCurrencyRates(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An Error occurred while syncing ECB currency rates. " + ex.Message);
        }
    }
}