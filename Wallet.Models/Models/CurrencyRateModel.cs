namespace Wallet.Models.Models;

public class CurrencyRate
{
    public DateTime Date { get; set; }
    public List<ExchangeRate> Rates { get; set; } = new();
}

public class ExchangeRate
{
    public string Currency { get; set; }
    public decimal Rate { get; set; }
}