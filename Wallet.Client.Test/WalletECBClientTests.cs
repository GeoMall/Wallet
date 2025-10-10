using System.Net;
using System.Xml;
using Moq;
using Moq.Protected;
using Wallet.Client.ECB;

namespace Wallet.Client.Test;

public class WalletEcbClientTests
{
    [Fact]
    public async Task WalletECBClient_ReturnsCorrectXML()
    {
        const string mockResponse =
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <gesmes:Envelope xmlns:gesmes="http://www.gesmes.org/xml/2002-08-01" xmlns="http://www.ecb.int/vocabulary/2002-08-01/eurofxref">
             <gesmes:subject>Reference rates</gesmes:subject>
             <gesmes:Sender>
                 <gesmes:name>European Central Bank</gesmes:name>
             </gesmes:Sender>
             <Cube>
                 <Cube time='2025-10-09'>
                     <Cube currency='USD' rate='1.1611'/>
                     <Cube currency='JPY' rate='177.40'/>
                     <Cube currency='BGN' rate='1.9558'/>
                     <Cube currency='CZK' rate='24.326'/>
                     <Cube currency='DKK' rate='7.4668'/>
                     <Cube currency='GBP' rate='0.86830'/>
                     <Cube currency='HUF' rate='390.98'/>
                     <Cube currency='PLN' rate='4.2548'/>
                     <Cube currency='RON' rate='5.0945'/>
                     <Cube currency='SEK' rate='10.9870'/>
                     <Cube currency='CHF' rate='0.9309'/>
                     <Cube currency='ISK' rate='141.80'/>
                     <Cube currency='NOK' rate='11.6240'/>
                     <Cube currency='TRY' rate='48.4464'/>
                     <Cube currency='AUD' rate='1.7600'/>
                     <Cube currency='BRL' rate='6.1877'/>
                     <Cube currency='CAD' rate='1.6198'/>
                     <Cube currency='CNY' rate='8.2698'/>
                     <Cube currency='HKD' rate='9.0334'/>
                     <Cube currency='IDR' rate='19227.64'/>
                     <Cube currency='ILS' rate='3.7684'/>
                     <Cube currency='INR' rate='103.1025'/>
                     <Cube currency='KRW' rate='1647.80'/>
                     <Cube currency='MXN' rate='21.2683'/>
                     <Cube currency='MYR' rate='4.8952'/>
                     <Cube currency='NZD' rate='2.0097'/>
                     <Cube currency='PHP' rate='67.553'/>
                     <Cube currency='SGD' rate='1.5048'/>
                     <Cube currency='THB' rate='37.817'/>
                     <Cube currency='ZAR' rate='19.8575'/>
                 </Cube>
             </Cube>
            </gesmes:Envelope>
            """;

        var client = new WalletsEcbClient(CreateMockHttpClient(mockResponse));

        var response = await client.GetCurrencyRates();

        Assert.Equal(new DateTime(2025, 10, 09), response.Date);
        Assert.Contains(response.Rates, r => r is { Currency: "USD", Rate: 1.1611m });
        Assert.Contains(response.Rates, r => r is { Currency: "JPY", Rate: 177.40m });
        Assert.Contains(response.Rates, r => r is { Currency: "BGN", Rate: 1.9558m });
        Assert.Contains(response.Rates, r => r is { Currency: "CZK", Rate: 24.326m });
        Assert.Contains(response.Rates, r => r is { Currency: "DKK", Rate: 7.4668m });
        Assert.Contains(response.Rates, r => r is { Currency: "GBP", Rate: 0.86830m });
        Assert.Contains(response.Rates, r => r is { Currency: "HUF", Rate: 390.98m });
        Assert.Contains(response.Rates, r => r is { Currency: "PLN", Rate: 4.2548m });
        Assert.Contains(response.Rates, r => r is { Currency: "RON", Rate: 5.0945m });
        Assert.Contains(response.Rates, r => r is { Currency: "SEK", Rate: 10.9870m });
        Assert.Contains(response.Rates, r => r is { Currency: "CHF", Rate: 0.9309m });
        Assert.Contains(response.Rates, r => r is { Currency: "ISK", Rate: 141.80m });
        Assert.Contains(response.Rates, r => r is { Currency: "NOK", Rate: 11.6240m });
        Assert.Contains(response.Rates, r => r is { Currency: "TRY", Rate: 48.4464m });
        Assert.Contains(response.Rates, r => r is { Currency: "AUD", Rate: 1.7600m });
        Assert.Contains(response.Rates, r => r is { Currency: "BRL", Rate: 6.1877m });
        Assert.Contains(response.Rates, r => r is { Currency: "CAD", Rate: 1.6198m });
        Assert.Contains(response.Rates, r => r is { Currency: "CNY", Rate: 8.2698m });
        Assert.Contains(response.Rates, r => r is { Currency: "HKD", Rate: 9.0334m });
        Assert.Contains(response.Rates, r => r is { Currency: "IDR", Rate: 19227.64m });
        Assert.Contains(response.Rates, r => r is { Currency: "ILS", Rate: 3.7684m });
        Assert.Contains(response.Rates, r => r is { Currency: "INR", Rate: 103.1025m });
        Assert.Contains(response.Rates, r => r is { Currency: "KRW", Rate: 1647.80m });
        Assert.Contains(response.Rates, r => r is { Currency: "MXN", Rate: 21.2683m });
        Assert.Contains(response.Rates, r => r is { Currency: "MYR", Rate: 4.8952m });
        Assert.Contains(response.Rates, r => r is { Currency: "NZD", Rate: 2.0097m });
        Assert.Contains(response.Rates, r => r is { Currency: "PHP", Rate: 67.553m });
        Assert.Contains(response.Rates, r => r is { Currency: "SGD", Rate: 1.5048m });
        Assert.Contains(response.Rates, r => r is { Currency: "THB", Rate: 37.817m });
        Assert.Contains(response.Rates, r => r is { Currency: "ZAR", Rate: 19.8575m });
    }

    [Fact]
    public async Task WalletECBClient_ReturnsInvalidXML()
    {
        const string mockResponse =
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <gesmes:Envelope xmlns:gesmes="http://www.gesmes.org/xml/2002-08-01" xmlns="http://www.ecb.int/vocabulary/2002-08-01/eurofxref">
             <gesmes:subject>Reference rates</gesmes:subject>
             <gesmes:Sender>
                 <gesmes:name>European Central Bank</gesmes:name>
             </gesmes:Sender>
             <Cube>
             </Cube>
            </gesmes:Envelope>
            """;

        var client = new WalletsEcbClient(CreateMockHttpClient(mockResponse));

        await Assert.ThrowsAsync<XmlException>(async () => await client.GetCurrencyRates());
    }

    private static HttpClient CreateMockHttpClient(string response)
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(response)
            });

        return new HttpClient(mockMessageHandler.Object);
    }
}