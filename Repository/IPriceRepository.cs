using CryptoFinAPI.Models;

namespace CryptoFinAPI.Repository;

/// <summary>
/// Interface with service methods for manage repository
/// </summary>
public interface IPriceRepository
{
    public Task<PricePoint> GetPriceAsync(bool isSeconds, string currencyPair, string start_date, CancellationToken token);
    public Task<IList<PricePoint>> GetPricesAsync(bool isSeconds, string currencyPair, string start_date, CancellationToken token);
    public Task<IList<PricePoint>> GetPriceForTimeRangeAsync(bool isSeconds, string currencyPair, string start_date, string end_date, CancellationToken token);
    public Task<int> InsertPriceAsync(PricePoint price, CancellationToken token);
    public Task<int> InsertRangePriceAsync(IList<PricePoint> prices, CancellationToken token);
}
