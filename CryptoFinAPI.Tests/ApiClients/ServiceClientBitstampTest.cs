using System.Net;
using CryptoFinAPI.ApiClients;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;

namespace CryptoFinAPI.UnitTests.ApiClients;

public class ServiceClientBitstampTest
{
    private readonly string _baseClientUrl;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly ServiceClientBitstamp _apiClient;

    public ServiceClientBitstampTest()
    {
        _baseClientUrl = "https://www.bitstamp.net/api/v2/ohlc/";
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _apiClient = new ServiceClientBitstamp(_httpClient);
    }

    [Fact]
    public async Task GetPriceAsync_ReturnsCorrectPrice()
    {
        // Arrange
        var currencyPair = "BTCUSD";
        var startTimePoint = "1672531200";
        var cancellationToken = CancellationToken.None;
        var limit = "1";
        var step = ServiceClientBitsFinex.ServicesParamStepDefault;

        var jsonResponse = new JObject
        {
            ["data"] = new JObject
            {
                ["ohlc"] = new JArray
                        {
                            new JObject
                            {
                                ["open"] = 1.0m,
                                ["high"] = 2.0m,
                                ["low"] = 0.5m,
                                ["close"] = 1.5m,
                                ["timestamp"] = startTimePoint
                            }
                        }
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
        var result = await _apiClient.GetPriceAsync(currencyPair, limit, step, startTimePoint, cancellationToken);

        // Assert
        Assert.Equal(currencyPair, result.CurrencyPair);
        Assert.Equal("1.0", result.Open);
        Assert.Equal("2.0", result.High);
        Assert.Equal("0.5", result.Low);
        Assert.Equal("1.5", result.Close);
        Assert.Equal(Utils.convertUnixSecondsToDateTime(startTimePoint), result.StartDate);
    }

    [Fact]
    public async Task GetPriceAsync_ThrowsException_OnHttpError()
    {
        // Arrange
        var currencyPair = "BTCUSD";
        var startTimePoint = "1672531200";
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
    public async Task GetPriceAsync_HandlesCancellationToken()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var currencyPair = "BTCUSD";
        var startTimePoint = "1672531200";
        var limit = "1";
        var step = ServiceClientBitsFinex.ServicesParamStepDefault;
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
}

