using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CryptoFinAPI.Models;

[ExcludeFromCodeCoverage]
[Table("prices")]
public class PricePoint : RuntimeModel
{
    [Column("id")]
    [Key]
    public int id { get; set; }

    [MaxLength(16)]
    [Column("currency_pair", Order = 2, TypeName = "nvarchar(11)")]
    public string CurrencyPair { get; set; } = string.Empty;

    [Column("interval", Order = 3, TypeName = "int")]
    public string? Interval { get; set; }

    [Column("open", Order = 4, TypeName = "decimal(18, 2)")]
    public string Open { get; set; } = string.Empty;

    [Column("high", Order = 5, TypeName = "decimal(18, 2)")]
    public string High { get; set; }

    [Column("low", Order = 6, TypeName = "decimal(18, 2)")]
    public string Low { get; set; }

    [Column("close", Order = 7, TypeName = "decimal(18, 2)")]
    public string Close { get; set; }

    [Column("start_date", Order = 8, TypeName = "datetime2(7)")]
    public DateTime StartDate { get; set; }

    [Column("end_date", Order = 9, TypeName = "datetime2(7)")]
    public DateTime? EndDate { get; set; }

    [Column("time_span", Order = 10, TypeName = "datetime2(7)")]
    public DateTime TimeSpan { get; set; }

    [Column("step", Order = 11, TypeName = "int")]
    public string? Step { get; set; }

    public static PricePoint Empty => new()
    {
        CurrencyPair = string.Empty
    };
}

