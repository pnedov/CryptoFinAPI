using CryptoFinAPI.Models;
using System.Threading;

namespace CryptoFinAPI.Services.Aggregators;

public class AggregateClosePrices : IPriceAggregator
{
    public async Task<decimal> AggregateAsync(IList<PricePoint> prices, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var list = prices.Select(b => decimal.Parse(b.Close)).ToList();

        return await Task.FromResult(list.Average());
    }
}

