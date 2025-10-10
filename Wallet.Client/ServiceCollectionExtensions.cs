using Microsoft.Extensions.DependencyInjection;
using Wallet.Client.ECB;

namespace Wallet.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWalletClients(this IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();
        services.AddSingleton<WalletsEcbClient>();

        return services;
    }
}