using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace CryptoFinAPI.Models;


[ExcludeFromCodeCoverage]
public class BitFinex
{
   public List<List<double>> Data { get; set; }
}


[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class Root
{
    public List<List<double>> Data { get; set; }
}

