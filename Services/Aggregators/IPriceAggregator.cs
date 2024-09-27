using CryptoFinAPI.Models;

namespace CryptoFinAPI.Services.Aggregators;

/// <summary>
/// Interface with service methods for calculate prices
/// </summary>
public interface IPriceAggregator
{
    public Task<decimal> AggregateAsync(IList<PricePoint> prices, CancellationToken token);
}

