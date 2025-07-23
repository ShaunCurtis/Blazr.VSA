/// ============================================================
/// Author: Shaun Curtis, old Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing record requests to get a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed class WeatherForecastEntityHandler : IRequestHandler<WeatherForecastEntityRequest, Result<WeatherForecastEntity>>
{
    private IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastEntityHandler(IDbContextFactory<InMemoryWeatherTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Result<WeatherForecastEntity>> HandleAsync(WeatherForecastEntityRequest request, CancellationToken cancellationToken)
    {
        var asyncResult = await _factory.CreateDbContext()
            .GetRecordAsync<DvoWeatherForecast>(new RecordQueryRequest<DvoWeatherForecast>(item => item.WeatherForecastID == request.Id.Value));
        //TODO - can we combine
        return asyncResult
            .ApplyTransform<DmoWeatherForecast>(WeatherForecastMap.Map)
            .ApplyTransform(WeatherForecastEntity.Load);
    }
}
