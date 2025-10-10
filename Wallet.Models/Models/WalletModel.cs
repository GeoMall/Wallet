using System.ComponentModel.DataAnnotations;

namespace Wallet.Service.Models;

public class WalletBase
{
    public required string UserId { get; set; }
    public string Currency { get; set; } = "EUR";
}

public class WalletRequest : WalletBase
{
}

public class WalletResponse : WalletBase
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}