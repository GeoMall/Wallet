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
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .StartNow());
        });

        services.AddQuartzHostedService(quartz => quartz.WaitForJobsToComplete = true);
    }
}