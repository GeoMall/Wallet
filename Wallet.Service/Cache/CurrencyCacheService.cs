using Microsoft.Extensions.Caching.Memory;
using Wallet.Database.Entities;

namespace Wallet.Service.Cache;

public interface ICurrencyCacheService
{
    Dictionary<string, decimal> GetCurrencyRates();
    Task SetCurrencyRates(Dictionary<string, decimal> rates);
    CurrencyRatesEntity GetCurrencyRateCode(string currencyCode);
}

public class CurrencyCacheService : ICurrencyCacheService
{
    private const string CacheKey = "CurrencyRates";
    private readonly IMemoryCache _cache;

    public CurrencyCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task SetCurrencyRates(Dictionary<string, decimal> rates)
    {
        _cache.Set(CacheKey, rates);

        return Task.CompletedTask;
    }

    public Dictionary<string, decimal> GetCurrencyRates()
    {
        _cache.TryGetValue(CacheKey, out Dictionary<string, decimal>? rates);

        return rates!;
    }

    public CurrencyRatesEntity GetCurrencyRateCode(string currencyCode)
    {
        var currencies = GetCurrencyRates();
        var rate = currencies[currencyCode];

        return new CurrencyRatesEntity { CurrencyCode = currencyCode, Rate = rate };
    }
}