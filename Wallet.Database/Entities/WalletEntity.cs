namespace Wallet.Database.Entities;

public class WalletEntity
{
    public Guid Id { get; set; }
    public required string CurrencyCode { get; set; }
    public decimal Balance { get; set; }
    public CurrencyEntity? Currency { get; set; }
}