using Microsoft.EntityFrameworkCore;
using Wallet.Client;
using Wallet.Database;
using Wallet.Scheduler;
using Wallet.Scheduler.Schedulers;
using Wallet.Service;

var builder = WebApplication.CreateBuilder(args);

// Connection string for MSSQL container
const string connectionString =
    "Server=localhost,1433;Database=WalletDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddWalletClients();
builder.Services.AddSchedulers();
builder.Services.AddScoped<WalletService>();
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
    context.Database.EnsureCreated();

    var scheduler = scope.ServiceProvider.GetRequiredService<IQuartzJobSchedulerConfiguration>();
    scheduler.Configure(builder.Services);
}

app.MapControllers();
app.Run();