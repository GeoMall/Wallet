using Microsoft.Extensions.DependencyInjection;
using Wallet.Service.Cache;
using Wallet.Service.Factories;
using Wallet.Service.Services;

namespace Wallet.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<StrategyFactory>();
        services.AddScoped<WalletService>();
        services.AddScoped<CurrencyService>();
        services.AddScoped<StrategyService>();

        services.AddSingleton<ICurrencyCacheService, CurrencyCacheService>();

        return services;
    }
}