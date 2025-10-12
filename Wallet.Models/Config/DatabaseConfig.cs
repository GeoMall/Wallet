namespace Wallet.Models.Config;

public class DatabaseConfig
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = string.Empty;
}