using System.Collections;
using CryptoFinAPI.Models;
using Newtonsoft.Json;

namespace CryptoFinAPI.ApiClients;

/// <summary>
/// Class for processing outside API https://www.bitstamp.net
/// </summary>
public class ServiceClientBitstamp(HttpClient httpClient) : IExternalApiClient
{
    private const string BaseclientUrl = "https://www.bitstamp.net/api/v2/ohlc/";
    private bool _isUnix = false;

    /// <summary>
    /// Flag for data timestamp format
    /// </summary>
    public bool isUnixTimeMilliseconds => _isUnix;

    /// <summary>
    /// Get single price object data from API response
    /// </summary>
    /// <param name="currencyPair">currency pair</param>
    /// <param name="limit">limit</param>
    /// <param name="startDate">start point</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="step">step in seconds</param>
    /// <returns></returns>
    public async Task<PricePoint> GetPriceAsync(string currencyPair, string limit, string step, string startDate, CancellationToken token)
    {
        PricePoint pricePoint = new();
        var url = buildClientURL(new Hashtable() {
                    { "currencyPair", currencyPair },
                    { "limit", limit },
                    { "start", startDate },
                    { "step", step }});

        var response = await httpClient.GetAsync(url, token);
        response.EnsureSuccessStatusCode();
        var model = JsonConvert.DeserializeObject<Bitstamp>(response.Content.ReadAsStringAsync(token).Result);

        if (model != null && model.Data.Ohlc.Count > 0)
        {
            pricePoint = castSingleType(model, currencyPair, startDate);
        }

        return await Task.FromResult(pricePoint);
    }

    /// <summary>
    /// Get multi price objects data from API response
    /// </summary>
    /// <param name="currencyPair">currency pair</param>
    /// <param name="limit">limit records </param>
    /// <param name="startDate">start datetime</param>
    /// <param name="endDate">end datetime</param>
    /// <param name="step">range result</param>
    /// <param name="token">cancellation token</param>
    /// <returns></returns>
    public Task<IList<PricePoint>> GetPriceForTimeRangeAsync(string currencyPair, string limit, string startDate, string endDate, string step, CancellationToken token)
    {
        IList<PricePoint> prices = [];
        var url = buildClientURL(new Hashtable() {
                    { "currencyPair", currencyPair },
                    { "limit", limit },
                    { "start", startDate },
                    { "step", step },
                    { "end", endDate }});
        using (var client = new HttpClient())
        {
            client.Timeout = new TimeSpan(0, 5, 0);
            using (var response = client.GetAsync(url, token).Result)
            {
                var resp_object = JsonConvert.DeserializeObject<Bitstamp>(response.Content.ReadAsStringAsync(token).Result);
                if (resp_object != null) return Task.FromResult(castMultiType(resp_object, currencyPair, startDate, endDate));
            }
        }

        return Task.FromResult(prices);
    }

    /// <summary>
    /// Build client API URI
    /// </summary>
    /// <param name="hs">URI params</param>
    /// <returns></returns>
    private string buildClientURL(Hashtable hs)
    {
        string url = BaseclientUrl + hs["currencyPair"];
        var query = new Dictionary<string, string>
        {
            { "start", Convert.ToString(hs["start"]) ?? string.Empty },
            { "end", Convert.ToString(hs["start"]) ?? string.Empty },
            { "limit", Convert.ToString(hs["limit"]) ?? string.Empty },
            { "step",  Convert.ToString(hs["step"])  ?? string.Empty }
        };

        return Utils.buildUrl(url, query);
    }

    /// <summary>
    /// Map response object to single model object
    /// </summary>
    /// <param name="objResult">response object</param>
    /// <param name="currencyPair">pair of currencies</param>
    /// <param name="startDate">start date</param>
    /// <returns></returns>
    private PricePoint castSingleType(Bitstamp objResult, string currencyPair, string startDate)
    {
        var new_obj = new PricePoint();
        var ohlc = objResult.Data.Ohlc;
        new_obj.High = ohlc.First().High;
        new_obj.Low = ohlc.First().Low;
        new_obj.Open = ohlc.First().Open;
        new_obj.Close = ohlc.First().Close;
        new_obj.CurrencyPair = currencyPair;
        new_obj.StartDate = Utils.convertUnixSecondsToDateTime(startDate);
        new_obj.TimeSpan = Utils.convertUnixSecondsToDateTime(ohlc.First().Timestamp);

        return new_obj;
    }

    /// <summary>
    /// Map response object to multi model object
    /// </summary>
    /// <param name="objResult">response object</param>
    /// <param name="currencyPair">pair of currencies</param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    private IList<PricePoint> castMultiType(Bitstamp objResult, string currencyPair, string startDate, string endDate)
    {
        var prices = new List<PricePoint>();
        foreach (var item in objResult.Data.Ohlc)
        {
            var new_obj = new PricePoint();
            new_obj.High = item.High;
            new_obj.Low = item.Low;
            new_obj.Open = item.Open;
            new_obj.Close = item.Close;
            new_obj.CurrencyPair = currencyPair;
            new_obj.TimeSpan = Utils.convertUnixSecondsToDateTime(item.Timestamp);
            new_obj.StartDate = Utils.convertUnixSecondsToDateTime(startDate);
            new_obj.EndDate = Utils.convertUnixSecondsToDateTime(endDate);
            prices.Add(new_obj);
        }

        return prices;
    }
}

