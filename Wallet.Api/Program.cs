using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wallet.Client;
using Wallet.Database;
using Wallet.Models;
using Wallet.Models.Config;
using Wallet.Scheduler;
using Wallet.Scheduler.Schedulers;
using Wallet.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddConfigOptions(builder.Configuration);

builder.Services.AddDbContext<WalletDbContext>((serviceProvider, options) =>
{
    var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfig>>().Value;
    options.UseSqlServer(dbConfig.ConnectionString);
});

builder.Services.AddWalletClients();
builder.Services.AddServices();
builder.Services.AddSchedulers();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitingConfig>(builder.Configuration.GetSection(IpRateLimitingConfig.SectionName));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var scheduler = new QuartzJobSchedulerConfiguration();
scheduler.Configure(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Wallet API v1");
    options.RoutePrefix = string.Empty;
});

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
    context.Database.EnsureCreated();
}

app.UseIpRateLimiting();
app.MapControllers();
app.Run();