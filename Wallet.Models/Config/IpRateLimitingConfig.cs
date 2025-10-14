namespace Wallet.Models.Config;

public class IpRateLimitingConfig
{
    public const string SectionName = "IpRateLimiting";

    public bool EnableEndpointRateLimiting { get; set; }
    public bool StackBlockedRequests { get; set; }
    public int HttpStatusCode { get; set; } = 429;
    public List<Rule> GeneralRules { get; set; } = [];
}

public class Rule
{
    public string Endpoint { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public int Limit { get; set; }
}