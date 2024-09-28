using System.Collections;
using System.Net;
using CryptoFinAPI.ApiClients;
using CryptoFinAPI.Models;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;

namespace CryptoFinAPI.UnitTests.ApiClients;

public class ServiceClientBitsFinexTest
{
    private readonly string _baseClientUrl;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly ServiceClientBitsFinex _apiClient;

    public ServiceClientBitsFinexTest()
    {
        _baseClientUrl = "https://api-pub.bitfinex.com/v2/candles/trade";
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _apiClient = new ServiceClientBitsFinex(_httpClient);
    }

    [Fact]
    public async Task GetPriceAsync_ReturnsCorrectPrice()
    {
        // Arrange
        var currencyPair = "BTCUSD";
        var startTimePoint = "1672531200000"; 
        var cancellationToken = CancellationToken.None;
        var limit = "1";
        var step = ServiceClientBitsFinex.ServicesParamStepDefault;
        PricePoint pricePoint = new();
        var url = buildClientURL(new Hashtable() {
                    { "currencyPair", currencyPair },
                    { "limit", limit },
                    { "start", startTimePoint },
                    { "step", step }});

        var jsonResponse = new JArray
            {
                new JArray
                {
                    "1672531200000",
                    1.0m, // Open
                    2.0m, // High
                    0.5m, // Low
                    1.5m  // Close
                }
            }.ToString();

        _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

        // Act
        var result = await _apiClient.GetPriceAsync(currencyPair, limit, step, startTimePoint.ToString(), cancellationToken);

        // Assert
        Assert.Equal(currencyPair, result.CurrencyPair);
        Assert.Equal("1", result.Open);
        Assert.Equal("0.5", result.High);
        Assert.Equal("1.5", result.Low);
        Assert.Equal("2", result.Close);
        Assert.Equal(Utils.convertUnixMillisecondsToDateTime(startTimePoint).Date, result.TimeSpan.Date);
    }

    [Fact]
    public async Task GetPriceAsync_ThrowsException_OnHttpError()
    {
        // Arrange
        var currencyPair = "BTCUSD";        
        var startTimePoint = "1672531200000";
        var limit = "1";
        var cancellationToken = CancellationToken.None;
        var step = ServiceClientBitsFinex.ServicesParamStepDefault;

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _apiClient.GetPriceAsync(currencyPair, limit, step, startTimePoint, cancellationToken));
    }

    [Fact]
    public async Task GetPriceAsync_ReturnsEmptyPricePoint_OnEmptyResponse()
    {
        // Arrange
        var currencyPair = "BTCUSD";
        var startTimePoint = "1672531200000";
        var limit = "1";
        var cancellationToken = CancellationToken.None;
        var step = ServiceClientBitsFinex.ServicesParamStepDefault;

        var jsonResponse = new JArray().ToString();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        PricePoint result = await _apiClient.GetPriceAsync(currencyPair, limit, step, startTimePoint, cancellationToken);

        // Assert
        Assert.Equal(string.Empty, result.CurrencyPair);
    }

    [Fact]
    public async Task GetPriceAsync_HandlesCancellationToken()
    {
        // Arrange
        var currencyPair = "BTCUSD";
        var startTimePoint = "1672531200000";
        var limit = "1";
        var step = ServiceClientBitsFinex.ServicesParamStepDefault;
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        cancellationTokenSource.Cancel();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => _apiClient.GetPriceAsync(currencyPair, limit, step, startTimePoint, cancellationToken));
    }

    /// <summary>
    /// Build client API URI
    /// </summary>
    /// <param name="hs">URI params</param>
    /// <returns></returns>
    private string buildClientURL(Hashtable hs)
    {
        string currencyPair = Convert.ToString(hs["currencyPair"]) ?? string.Empty;
        string url = $"{_baseClientUrl}:1h:t{currencyPair.ToUpper()}/hist";
        var query = new Dictionary<string, string>
        {
            { "start", Convert.ToString(hs["start"]) ?? string.Empty },
            { "end",   Convert.ToString(hs["end"]) ?? string.Empty },
            { "limit", Convert.ToString(hs["limit"]) ?? string.Empty }
        };

        return Utils.buildUrl(url, query);
    }
}

