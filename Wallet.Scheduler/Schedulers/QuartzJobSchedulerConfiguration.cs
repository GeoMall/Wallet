using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Wallet.Scheduler.Jobs;

namespace Wallet.Scheduler.Schedulers;

public interface IQuartzJobSchedulerConfiguration
{
    void Configure(IServiceCollection services);
}

public class QuartzJobSchedulerConfiguration : IQuartzJobSchedulerConfiguration
{
    public void Configure(IServiceCollection services)
    {
        services.AddQuartz(quartz =>
        {
            var jobKey = new JobKey("CurrencyRateUpdateJob");

            quartz.AddJob<CurrencyUpdaterJob>(options => options.WithIdentity(jobKey));

            quartz.AddTrigger(options => options
                .ForJob(jobKey)
                .WithIdentity("CurrencyRateUpdateJob-Trigger")
                .WithCronSchedule("0 0 * * * ?")
                .StartNow());
        });

        services.AddQuartzHostedService(quartz => quartz.WaitForJobsToComplete = true);
    }
}