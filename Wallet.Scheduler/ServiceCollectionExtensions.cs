using Microsoft.Extensions.DependencyInjection;
using Wallet.Scheduler.Jobs;
using Wallet.Scheduler.Schedulers;

namespace Wallet.Scheduler;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSchedulers(this IServiceCollection services)
    {
        services.AddScoped<CurrencyUpdaterJob>();
        services.AddSingleton<IQuartzJobSchedulerConfiguration, QuartzJobSchedulerConfiguration>();

        return services;
    }
}