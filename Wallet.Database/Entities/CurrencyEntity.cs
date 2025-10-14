namespace Wallet.Database.Entities;

public class CurrencyEntity
{
    public Guid Id { get; set; }
    public required string CurrencyCode { get; set; }

    // Navigation property - one currency can have many Wallets
    public ICollection<WalletEntity>? Wallets { get; set; }

    // Navigation property - one currency can have many Currency Rates
    public ICollection<CurrencyRatesEntity>? Currency { get; set; }
}