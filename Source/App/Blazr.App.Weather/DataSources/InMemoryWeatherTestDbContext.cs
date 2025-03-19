/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Infrastructure.Server;
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Weather.Infrastructure;

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

    public async ValueTask<Result<TRecord>> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request, CancellationToken cancellationToken = new())
        where TRecord : class
    {
        return await DbBroker<InMemoryWeatherTestDbContext>.ExecuteCommandAsync<TRecord>(this, request, cancellationToken);
    }

    public async ValueTask<Result<ListItemsProvider<TRecord>>> GetItemsAsync<TRecord>(ListQueryRequest<TRecord> request)
    where TRecord : class
    {
        return await DbBroker<InMemoryWeatherTestDbContext>.GetItemsAsync<TRecord>(this, request);
    }

    public async ValueTask<Result<TRecord>> GetRecordAsync<TRecord>(RecordQueryRequest<TRecord> request)
    where TRecord : class
    {
        return await DbBroker<InMemoryWeatherTestDbContext>.GetRecordAsync<TRecord>(this, request);
    }
}
