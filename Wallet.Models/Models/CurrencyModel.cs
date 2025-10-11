namespace Wallet.Models.Models;

public record CurrencyResponse
{
    public string Currency { get; set; }
    public decimal Rate { get; set; }
}