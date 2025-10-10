using Microsoft.EntityFrameworkCore;
using Wallet.Database.Entities;
using Wallet.Service.Models;

namespace Wallet.Database;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<WalletResponse> Wallets { get; set; }
    public DbSet<CurrencyEntity> CurrencyRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WalletResponse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Balance)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<CurrencyEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CurrencyCode).IsUnique();
            entity.Property(e => e.Rate).HasPrecision(18, 4);
        });
    }
}