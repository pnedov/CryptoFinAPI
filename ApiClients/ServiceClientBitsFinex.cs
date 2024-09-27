using CryptoFinAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace CryptoFinAPI.ApiClients;

/// <summary>
/// Class for processing outside API https://api-pub.bitfinex.com/v2
/// </summary>
public class ServiceClientBitsFinex(HttpClient httpClient) : IExternalApiClient
{
    public static string ServicesParamStepDefault = "3600";
    private const string BaseClientUrl = "https://api-pub.bitfinex.com/v2/candles/trade";
    private bool _isUnix = true;

    /// <summary>
    /// Flag for data timestamp format
    /// </summary>
    public bool isUnixTimeMilliseconds => _isUnix;

    /// <summary>
    /// Get single price object from client API for time point
    /// </summary>
    /// <param name="currencyPair">currency pair</param>
    /// <param name="limit">limit</param>
    /// <param name="startDate">timespan</param>
    /// <param name="token"></param>
    /// <param name="step">time period in seconds</param>
    /// <returns></returns>
    public async Task<PricePoint> GetPriceAsync(string currencyPair, string limit, string step, string startDate, CancellationToken token)
    {
        step = string.IsNullOrEmpty(step) ? ServiceClientBitsFinex.ServicesParamStepDefault : step;
        PricePoint pricePoint = new();
        var url = buildClientURL(new Hashtable() {
                    { "currencyPair", currencyPair },
                    { "limit", limit },
                    { "start", startDate },
                    { "step", step }});

        var response = await httpClient.GetAsync(url, token);
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync(token);
        var json = JArray.Parse(content);

        if (json.Count == 0)
        {
            return PricePoint.Empty;
        }

        pricePoint = castSingleType(content, currencyPair, startDate);
        response.EnsureSuccessStatusCode();

        return await Task.FromResult(pricePoint);
    }

    /// <summary>
    /// Get multi prices from client API for time range
    /// </summary>
    /// <param name="currencyPair">currency pair</param>
    /// <param name="limit">number of records (Max: 10000)</param>
    /// <param name="startDate">milliseconds start time</param>
    /// <param name="endDate">milliseconds end time</param>
    /// <param name="step">step</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IList<PricePoint>> GetPriceForTimeRangeAsync(string currencyPair, string limit, string startDate, string endDate, string step, CancellationToken token)
    {
        IList<PricePoint> prices = [];
        var url = buildClientURL(new Hashtable() {
                                    { "currencyPair", currencyPair },
                                    { "limit", limit },
                                    { "start", startDate },
                                    { "step", step },
                                    { "end", endDate }});

        var response = await httpClient.GetAsync(url, token);
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync(token);
        var json = JArray.Parse(content);

        if (json.Count == 0)
        {
            return prices;
        }

        prices = castMultiType(content, currencyPair, startDate, endDate);

        return await Task.FromResult(prices);
    }

    /// <summary>
    /// Build client API URI
    /// </summary>
    /// <param name="hs">URI params</param>
    /// <returns></returns>
    public string buildClientURL(Hashtable hs)
    {
        string currencyPair = Convert.ToString(hs["currencyPair"]) ?? string.Empty;
        string url = $"{BaseClientUrl}:1h:t{currencyPair.ToUpper()}/hist";
        var query = new Dictionary<string, string>
        {
            { "start", Convert.ToString(hs["start"]) ?? string.Empty },
            { "end",   Convert.ToString(hs["end"]) ?? string.Empty },
            { "limit", Convert.ToString(hs["limit"]) ?? string.Empty }
        };

        return Utils.buildUrl(url, query);
    }

    /// <summary>
    /// Map response object to single model object
    /// </summary>
    /// <param name="respObject">response object</param>
    /// <param name="currencyPair"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    private PricePoint castSingleType(string respObject, string currencyPair, string startDate)
    {
        var result = JsonConvert.DeserializeObject<List<List<double>>>(respObject);
        PricePoint newObj = new();
        if (result != null && result.Count > 0)
        {
            var list = result[0];
            newObj.High = list[3].ToString();
            newObj.Low = list[4].ToString();
            newObj.Open = list[1].ToString();
            newObj.Close = list[2].ToString();
            newObj.CurrencyPair = currencyPair;
            newObj.TimeSpan = Utils.convertUnixMillisecondsToDateTime(list[0].ToString());
            newObj.StartDate = Utils.convertUnixMillisecondsToDateTime(startDate);
        }
            return newObj;
    }

    /// <summary>
    /// Map response object to multi model object
    /// </summary>
    /// <param name="respObject">response object</param>
    /// <param name="currencyPair">pair currencies name</param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    IList<PricePoint> castMultiType(string respObject, string currencyPair, string startDate, string endDate)
    {
        var result = JsonConvert.DeserializeObject<List<List<double>>>(respObject);
        List<PricePoint> list = new();
        if (result != null && result.Count > 0)
        {
            foreach (var item in result)
            {
                var newObj = new PricePoint();
                newObj.High = item[3].ToString();
                newObj.Low = item[4].ToString();
                newObj.Open = item[1].ToString();
                newObj.Close = item[2].ToString();
                newObj.CurrencyPair = currencyPair;
                newObj.TimeSpan = Utils.convertUnixMillisecondsToDateTime(item[0].ToString());
                newObj.StartDate = Utils.convertUnixMillisecondsToDateTime(startDate);
                newObj.EndDate = Utils.convertUnixMillisecondsToDateTime(endDate);
                list.Add(newObj);
            }
        }

        return list;
    }
}
