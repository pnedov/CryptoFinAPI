using CryptoFinAPI.ApiClients;
using CryptoFinAPI.Models;
using CryptoFinAPI.Repository;
using CryptoFinAPI.Services;
using CryptoFinAPI.Services.Aggregators;

namespace CryptoFinAPI.App_Code;

public class PriceService : IPriceService
{
    private IList<IExternalApiClient> _clients;
    private IPriceRepository _repo;
    private const int UnixMinvalueDate = 1970;

    public PriceService(IPriceRepository repo)
    {
       _repo = repo;
       _clients = new List<IExternalApiClient>() { new ServiceClientBitstamp(new HttpClient()), new ServiceClientBitsFinex(new HttpClient()) };
    }

    /// <summary>
    /// Get prices from DB or from client API response and calculate price
    /// </summary>
    /// <param name="currencyPair">currency pair</param>
    /// <param name="step">time period in seconds</param>
    /// <param name="limit">limit records for API response</param>
    /// <param name="token"></param>
    /// <param name="startDate">time point</param>
    /// <returns></returns>
    public async Task<decimal> GetPriceAtTimeAsync(string currencyPair, string startDate, string step, string limit, CancellationToken token)
    {
        DateTimeOffset dtOffset = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(startDate));
        bool isSeconds = dtOffset.Year == UnixMinvalueDate;

        // qick fix
        step = (string.IsNullOrEmpty(step)) ? ServiceClientBitsFinex.ServicesParamStepDefault : step;
        var processor = new AggregateClosePrices();

        IList<PricePoint> prices = [];
        prices = await _repo.GetPricesAsync(isSeconds, currencyPair, startDate, token);

        if (prices != null && prices.Count > 0)
        {
            return await processor.AggregateAsync(prices, token);
        }

        // db has not record for required timespan and currency pair 
        foreach (IExternalApiClient clientAPI in _clients)
        {
            if (isSeconds && clientAPI.isUnixTimeMilliseconds)
            {
                Utils.convertToMilliseconds2(ref startDate);
            }
            if (!isSeconds && !clientAPI.isUnixTimeMilliseconds)
            {
                Utils.convertToSeconds2(ref startDate);
            }

            var price = clientAPI.GetPriceAsync(currencyPair, limit, step, startDate, token);
            await _repo.InsertPriceAsync(price.Result, token);
            prices.Add(price.Result);
        }

        return await processor.AggregateAsync(prices, token);
    }

    /// <summary>
    /// Get prices for range dates
    /// </summary>
    /// <param name="currencyPair">currency pair</param>
    /// <param name="limit"></param>
    /// <param name="startDate">start date</param>
    /// <param name="endDate"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public async Task<IList<PricePoint>> GetPriceForTimeRangeAsync(string currencyPair, string limit, string startDate, string endDate, string step, CancellationToken token)
    {
        DateTimeOffset dtOffset = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(startDate));
        bool isSeconds = dtOffset.Year == UnixMinvalueDate;
        List<PricePoint> prices = new();

        var prices_repo = _repo.GetPriceForTimeRangeAsync(isSeconds, currencyPair, startDate, endDate, token);
        prices.AddRange(prices_repo.Result);

        if (prices != null && prices.Count == 0)
        {
            foreach (IExternalApiClient clientAPI in _clients)
            {
                if (isSeconds && clientAPI.isUnixTimeMilliseconds)
                {
                    Utils.convertToMillisecondsRange(ref startDate, ref endDate);
                }
                if (!isSeconds && !clientAPI.isUnixTimeMilliseconds)
                {
                    Utils.convertToSeconds(ref startDate, ref endDate);
                }

                var rangePrices = await clientAPI.GetPriceForTimeRangeAsync(currencyPair, limit, startDate, endDate, step, token);
                prices.AddRange(rangePrices);
            }
        }
        else if(prices != null && prices.Count > 0) 
        {
            return prices;
        }

        if (prices != null && prices.Count > 0)
        {
           var result = _repo.InsertRangePriceAsync(prices, token);
        }

        return prices ?? new List<PricePoint>();
    }
}
