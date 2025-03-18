/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Infrastructure;

public sealed class InMemoryWeatherTestDbContext
    : DbContext
{
    public DbSet<DboWeatherForecast> DboWeatherForecasts { get; set; } = default!;
    public DbSet<DvoWeatherForecast> DvoWeatherForecasts { get; set; } = default!;

    public InMemoryWeatherTestDbContext(DbContextOptions<InMemoryWeatherTestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboWeatherForecast>().ToTable("WeatherForecasts");
        modelBuilder.Entity<DvoWeatherForecast>()
            .ToInMemoryQuery(()
                => from w in this.DboWeatherForecasts
                   select new DvoWeatherForecast
                   {
                       WeatherForecastID = w.WeatherForecastID,
                        Summary = w.Summary,
                         Temperature = w.Temperature,
                       Date = w.Date,
                   }).HasKey(x => x.WeatherForecastID);

    }
}
