using CryptoFinAPI.Models;

namespace CryptoFinAPI.Services;

/// <summary>
/// Interface with requests API service methods
/// </summary>
public interface IPriceService
{
    public Task<decimal> GetPriceAtTimeAsync(string currencyPair, string limit, string step, string at, CancellationToken token);
    public Task<IList<PricePoint>> GetPriceForTimeRangeAsync(string currencyPair, string limit, string from, string to, string step, CancellationToken token);
}

