using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Wallet.Models.Models;

namespace Wallet.Client.ECB;

public class WalletsEcbClient
{
    private const string BaseUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
    private readonly HttpClient _httpClient;

    public WalletsEcbClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CurrencyRate> GetCurrencyRates()
    {
        var response = await _httpClient.GetStringAsync(BaseUrl);

        return ParseXml(response);
    }

    private static CurrencyRate ParseXml(string xmlResponse)
    {
        var xdoc = XDocument.Parse(xmlResponse);

        var timeCube = xdoc.Descendants()
            .FirstOrDefault(x => x.Attribute("time") != null);

        if (timeCube == null)
            throw new XmlException("ECB XML format unexpected.");

        var date = DateTime.Parse(timeCube.Attribute("time")!.Value);

        var rates = timeCube.Elements()
            .Select(x => new ExchangeRate
            {
                Currency = x.Attribute("currency")!.Value,
                Rate = decimal.Parse(x.Attribute("rate")!.Value,
                    CultureInfo.InvariantCulture)
            })
            .ToList();

        return new CurrencyRate
        {
            Date = date,
            Rates = rates
        };
    }
}