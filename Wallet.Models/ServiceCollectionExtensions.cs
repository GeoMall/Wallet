using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wallet.Models.Config;

namespace Wallet.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfig>(configuration.GetSection(DatabaseConfig.SectionName));

        services.AddSingleton(resolver =>
            resolver.GetRequiredService<IOptions<DatabaseConfig>>().Value
        );

        //TODO ADD QUARTZ SETTINGS HERE

        return services;
    }
}