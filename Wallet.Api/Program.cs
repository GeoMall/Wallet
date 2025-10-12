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
builder.Services.AddServices();
builder.Services.AddSchedulers();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var scheduler = new QuartzJobSchedulerConfiguration();
scheduler.Configure(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Wallet API v1");
    options.RoutePrefix = string.Empty; // Makes Swagger UI open at root (http://localhost:5108)
});

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
    context.Database.EnsureCreated();
}

app.MapControllers();
app.Run();