using Microsoft.EntityFrameworkCore;
using Wallet.Database.Entities;

namespace Wallet.Database;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<WalletEntity> Wallets { get; set; }
    public DbSet<CurrencyRatesEntity> CurrencyRates { get; set; }
    public DbSet<CurrencyEntity> CurrencyCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CurrencyEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrencyCode);
        });

        modelBuilder.Entity<WalletEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Balance)
                .HasPrecision(18, 2);

            //Relationship with Currency Entity
            entity
                .HasOne(w => w.Currency)
                .WithMany(c => c.Wallets)
                .HasForeignKey(w => w.CurrencyCode)
                .HasPrincipalKey(c => c.CurrencyCode)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CurrencyCode);
        });

        modelBuilder.Entity<CurrencyRatesEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(cr => cr.Currency) // each CurrencyRate has one Currency
                .WithMany(c => c.Currency) // each Currency has many CurrencyRates
                .HasForeignKey(cr => cr.CurrencyCode) // FK property
                .HasPrincipalKey(c => c.CurrencyCode); // PK in Currency

            entity.HasIndex(e => new { e.CurrencyCode, e.ConversionDate })
                .IsUnique();
            entity.Property(e => e.Rate).HasPrecision(18, 4);
        });
    }
}