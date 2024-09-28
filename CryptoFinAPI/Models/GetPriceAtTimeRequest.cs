using System.Diagnostics.CodeAnalysis;

namespace CryptoFinAPI.Models;

[ExcludeFromCodeCoverage]
public class GetPriceAtTimeRequest
{
    /// <summary>
    /// The currency pair to get the price for (e.g., BTCUSD")
    /// </summary>
    public string CurrencyPair { get; set; }

    /// <summary>
    /// Limit results.
    /// </summary>
    public string Limit { get; set; }

    /// <summary>
    /// Timeframe in seconds.
    /// </summary>
    public string Step { get; set; }

    /// <summary>
    /// The date and time to get the price for (e.g., 2023-01-01T00:00:00)
    /// </summary>
    public string Start { get; set; }
}

