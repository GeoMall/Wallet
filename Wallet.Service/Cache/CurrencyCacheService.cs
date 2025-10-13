using Microsoft.Extensions.Caching.Memory;
using Wallet.Database.Entities;
using Wallet.Models.Models;
using Wallet.Service.Services;

namespace Wallet.Service.Cache;

public interface ICurrencyCacheService
{
    Task<Dictionary<string, decimal>> GetCurrencyRates();
    Task SetCurrencyRates(CurrencyRateResponse rates);
    Task<CurrencyEntity> GetCurrencyRateCode(string currencyCode);
}

public class CurrencyCacheService : ICurrencyCacheService
{
    private const string CacheKey = "CurrencyRates";
    private readonly IMemoryCache _cache;
    private readonly CurrencyService _currencyService;

    public CurrencyCacheService(IMemoryCache cache, CurrencyService currencyService)
    {
        _cache = cache;
        _currencyService = currencyService;
    }

    public Task SetCurrencyRates(CurrencyRateResponse rates)
    {
        var cacheDict = rates.Rates.ToDictionary(
            k => k.Currency,
            v => v.Rate
        );

        _cache.Set(CacheKey, cacheDict);

        return Task.CompletedTask;
    }

    public async Task<Dictionary<string, decimal>> GetCurrencyRates()
    {
        if (_cache.TryGetValue(CacheKey, out Dictionary<string, decimal>? rates))
            return rates!;

        //fallback action to retrieve from db and repopulate cache
        var currencyRates = await _currencyService.GetAllCurrencies();
        _cache.Set(CacheKey, currencyRates);

        return currencyRates;
    }

    public async Task<CurrencyEntity> GetCurrencyRateCode(string currencyCode)
    {
        var currencies = await GetCurrencyRates();
        var rate = currencies[currencyCode];

        return new CurrencyEntity { CurrencyCode = currencyCode, Rate = rate };
    }
}