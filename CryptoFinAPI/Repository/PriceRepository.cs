using CryptoFinAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoFinAPI.Repository;

public class PriceRepository : IPriceRepository
{
    private PriceDbContext _context;

    /// <summary>
    /// initialize DbContext object
    /// </summary>
    /// <param name="_context"></param>
    public PriceRepository(PriceDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get full data for price
    /// </summary>
    /// <param name="isSeconds"></param>
    /// <param name="currencyPair">currency pair</param>
    /// <param name="start">time point</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<PricePoint?> GetPriceAsync(bool isSeconds, string currencyPair, string start, CancellationToken token)
    {
        var dates = ConvertToDateTime(isSeconds, start, string.Empty);
        return await _context.Prices.SingleOrDefaultAsync(x => x.StartDate == dates[0] && x.CurrencyPair == currencyPair, token);
    }

    /// <summary>
    /// Get prices list by params
    /// </summary>
    /// <param name="isSeconds"></param>
    /// <param name="currencyPair">currency pair</param>
    /// <param name="start">time point</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IList<PricePoint>> GetPricesAsync(bool isSeconds, string currencyPair, string start, CancellationToken token)
    {
        var dates = ConvertToDateTime(isSeconds, start, string.Empty);
        return await _context.Prices.Where(x => x.StartDate == dates[0] && x.CurrencyPair == currencyPair).ToListAsync(token);
    }

    /// <summary>
    /// Get list prices by time range
    /// </summary>
    /// <param name="currencyPair">Currency pair</param>
    /// <param name="isSeconds">Flag of timestamp</param>
    /// <param name="from">Start point time</param>
    /// <param name="to">End point time</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IList<PricePoint>?> GetPriceForTimeRangeAsync(bool isSeconds, string currencyPair, string from, string to, CancellationToken token)
    {
        var dates = ConvertToDateTime(isSeconds, from, to);
        return await _context.Prices.Where(x => x.StartDate >= dates[0] &&
                                               x.EndDate <= dates[1] &&
                                               x.CurrencyPair == currencyPair).ToListAsync(token);
    }

    /// <summary>
    /// Save price object
    /// </summary>
    /// <param name="price"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<int> InsertPriceAsync(PricePoint price, CancellationToken token)
    {
        _context.Add(price);

        return await _context.SaveChangesAsync(token);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prices"></param>
    /// <returns></returns>
    public async Task<int> InsertRangePriceAsync(IList<PricePoint> prices, CancellationToken token)
    {
        _context.AddRangeAsync(prices, token);

        return await _context.SaveChangesAsync(token);
    }

    /// <summary>
    /// Convert string timestamp to type DateTime
    /// </summary>
    /// <param name="isSeconds"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private DateTime[] ConvertToDateTime(bool isSeconds, string from, string to)
    {
        DateTime dfrom = DateTime.UtcNow;
        DateTime dto = DateTime.UtcNow;

        if (isSeconds)
        {
            if (!string.IsNullOrEmpty(from))
            {
                dfrom = Utils.convertUnixSecondsToDateTime(from);
            }

            if (!string.IsNullOrEmpty(to))
            {
                dto = Utils.convertUnixSecondsToDateTime(to);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(from))
            {
                dfrom = Utils.convertUnixMillisecondsToDateTime(from);
            }

            if (!string.IsNullOrEmpty(to))
            {
                dto = Utils.convertUnixMillisecondsToDateTime(to);
            }
        }

        return new DateTime[] { dfrom, dto };
    }
}