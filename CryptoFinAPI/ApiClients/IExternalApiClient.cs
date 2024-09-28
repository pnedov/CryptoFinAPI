using CryptoFinAPI.Models;

namespace CryptoFinAPI.ApiClients;

/// <summary>
/// Interface with service methods for getting price
/// </summary>
public interface IExternalApiClient
{
    public Task<PricePoint> GetPriceAsync(string currencyPair, string limit, string step, string timespan, CancellationToken token);
    public Task<IList<PricePoint>> GetPriceForTimeRangeAsync(string currencyPair, string limit, string start, string end, string step, CancellationToken token);
    public bool isUnixTimeMilliseconds { get; }
}

