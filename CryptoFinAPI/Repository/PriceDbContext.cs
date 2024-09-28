using Microsoft.EntityFrameworkCore;
using CryptoFinAPI.Models;

namespace CryptoFinAPI.Repository;

public class PriceDbContext : DbContext
{
    protected readonly IConfiguration _configuration;

    public PriceDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PricePoint>(e =>
        {
            e.HasKey(k => k.id);
            e.HasIndex(e => e.CurrencyPair).IsClustered();
        });

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_configuration.GetConnectionString("WebApiDB"), option =>
        {
            option.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName);
        });

        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<PricePoint> Prices { get; set; }
}

