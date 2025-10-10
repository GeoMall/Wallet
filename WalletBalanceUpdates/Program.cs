using Wallet.Client.ECB;

namespace Wallet;

public class Program
{
    public static async Task Main(string[] args)
    {
        var client = new WalletsEcbClient(new HttpClient());

        var response = await client.GetCurrencyRates();
    }
}