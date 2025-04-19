/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.EntityFramework;

/// <summary>
/// Mediatr Handler for executing record requests to get a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed class WeatherForecastRecordHandler : IRequestHandler<WeatherForecastRecordRequest, Result<DmoWeatherForecast>>
{
    private IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastRecordHandler(IDbContextFactory<InMemoryWeatherTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Result<DmoWeatherForecast>> HandleAsync(WeatherForecastRecordRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        Expression<Func<DvoWeatherForecast, bool>> findExpression = (item) =>
            item.WeatherForecastID == request.Id.Value;

        var query = new RecordQueryRequest<DvoWeatherForecast>(findExpression);

        var result = await dbContext.GetRecordAsync<DvoWeatherForecast>(query);

        if (result.HasNotSucceeded(out DvoWeatherForecast? record))
            return result.ConvertFail<DmoWeatherForecast>();

        var returnItem = WeatherForecastMap.Map(record);

        return Result<DmoWeatherForecast>.Success(returnItem);
    }
}
