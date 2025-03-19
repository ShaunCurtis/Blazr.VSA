/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Weather.Infrastructure;

public sealed class InMemoryWeatherTestDbContext
    : DbContext
{
    public DbSet<DboWeatherForecast> DboWeatherForecasts { get; set; } = default!;
    public DbSet<DvoWeatherForecast> DvoWeatherForecasts { get; set; } = default!;
    public DbSet<DboUser> DboUsers { get; set; } = default!;
    public DbSet<DvoUser> DvoUsers { get; set; } = default!;

    public InMemoryWeatherTestDbContext(DbContextOptions<InMemoryWeatherTestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboWeatherForecast>().ToTable("WeatherForecasts");
        modelBuilder.Entity<DvoWeatherForecast>()
            .ToInMemoryQuery(()
                => from w in this.DboWeatherForecasts
                   join u in this.DboUsers on w.OwnerID equals u.UserID
                   select new DvoWeatherForecast
                   {
                       WeatherForecastID = w.WeatherForecastID,
                       Summary = w.Summary,
                       Temperature = w.Temperature,
                       Date = w.Date,
                       OwnerID = w.OwnerID,
                       Owner = u.Name,
                   }).HasKey(x => x.WeatherForecastID);

        modelBuilder.Entity<DboUser>().ToTable("Users");
        modelBuilder.Entity<DvoUser>()
            .ToInMemoryQuery(()
                => from w in this.DboUsers
                   select new DvoUser
                   {
                       UserID = w.UserID,
                       Name = w.Name,
                       Role = w.Role,
                   }).HasKey(x => x.UserID);

    }
}
