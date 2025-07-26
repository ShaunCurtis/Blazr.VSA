/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing record requests to get a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed class WeatherForecastRecordHandler : IRequestHandler<WeatherForecastRecordRequest, Result<DmoWeatherForecast>>
{
    private IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastRecordHandler(IDbContextFactory<InMemoryWeatherTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public Task<Result<DmoWeatherForecast>> HandleAsync(WeatherForecastRecordRequest request, CancellationToken cancellationToken)
        => _factory.CreateDbContext()
                .GetRecordAsync<DvoWeatherForecast>(new RecordQueryRequest<DvoWeatherForecast>(item => item.WeatherForecastID == request.Id.Value))
                .ApplyTransformAsync(WeatherForecastMap.Map);

}
