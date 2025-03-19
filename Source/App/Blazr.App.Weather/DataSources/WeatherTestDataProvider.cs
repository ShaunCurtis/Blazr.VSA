/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Weather.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Infrastructure;

public sealed class WeatherTestDataProvider
{
    public IEnumerable<DboWeatherForecast> WeatherForecasts => _weatherForecasts.AsEnumerable();

    private List<DboWeatherForecast> _weatherForecasts = new List<DboWeatherForecast>();

    public WeatherTestDataProvider()
    {
        this.Load();
    }

    private void Load()
    {
        var startDate = DateTime.Now;
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        _weatherForecasts = Enumerable.Range(1, 50).Select(index => new DboWeatherForecast
        {
            WeatherForecastID = Guid.CreateVersion7(),
            OwnerID = new Guid($"10000000-0000-0000-0000-20000000000{Random.Shared.Next(1, 3)}"),
            Date = startDate.AddDays(index),
            Temperature = new(Random.Shared.Next(-20, 55)),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToList();
    }

    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var dboWeatherForecasts = dbContext.Set<DboWeatherForecast>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again

        if (dboWeatherForecasts.Count() == 0)
            dbContext.AddRange(_weatherForecasts);

        dbContext.SaveChanges();
    }

    private static WeatherTestDataProvider? _provider;

    public static WeatherTestDataProvider Instance()
    {
        if (_provider is null)
            _provider = new WeatherTestDataProvider();

        return _provider;
    }
}