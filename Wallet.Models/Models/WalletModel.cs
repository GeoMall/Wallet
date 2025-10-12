namespace Wallet.Models.Models;

public record WalletRequest
{
    public string Currency { get; set; }
}

public record WalletCreateResponse
{
    public Guid Id { get; set; }
}

public record WalletBase
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
}

public record WalletResponse : WalletBase
{
    public string Currency { get; set; }
}

public record AdjustBalanceWalletResponse : WalletBase
{
}