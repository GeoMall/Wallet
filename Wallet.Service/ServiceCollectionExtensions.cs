using Microsoft.Extensions.DependencyInjection;

namespace Wallet.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<WalletService>();
        services.AddScoped<CurrencyService>();

        return services;
    }
}