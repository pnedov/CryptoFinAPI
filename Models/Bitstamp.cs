using System.Diagnostics.CodeAnalysis;

namespace CryptoFinAPI.Models;

[ExcludeFromCodeCoverage]
public class Data
{
    public string Pair { get; set; }
    public List<Ohlc> Ohlc { get; set; }
}

public class Ohlc
{
    public string Timestamp { get; set; }
    public string Open { get; set; }
    public string High { get; set; }
    public string Low { get; set; }
    public string Close { get; set; }
    public string Volume { get; set; }
}

public class Bitstamp
{
    public Data Data { get; set; }
}


