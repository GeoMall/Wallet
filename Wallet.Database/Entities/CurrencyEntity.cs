namespace Wallet.Database.Entities;

public class CurrencyEntity
{
    public Guid Id { get; set; }
    public required string CurrencyCode { get; set; }
    public DateTime ConversionDate { get; set; }
    public decimal Rate { get; set; }
}