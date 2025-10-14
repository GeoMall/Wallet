using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Models.Config;

namespace Wallet.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfig>(configuration.GetSection(DatabaseConfig.SectionName));
        services.Configure<IpRateLimitingConfig>(configuration.GetSection(IpRateLimitingConfig.SectionName));

        return services;
    }
}