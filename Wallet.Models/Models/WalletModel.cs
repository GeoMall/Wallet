namespace Wallet.Models.Models;

public record WalletRequest
{
    public string Currency { get; set; }
}

public record WalletCreateResponse
{
    public Guid Id { get; set; }
}

public record WalletResponse
{
    public Guid Id { get; set; }
    public string Currency { get; set; }
    public decimal Balance { get; set; }
}