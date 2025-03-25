/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.EntityFramework;

/// <summary>
/// Mediatr Handler for executing record requests to get a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed class WeatherForecastAPIRecordHandler : IRequestHandler<WeatherForecastRecordRequest, Result<DmoWeatherForecast>>
{
    private IHttpClientFactory _factory;

    public WeatherForecastAPIRecordHandler(IHttpClientFactory dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Result<DmoWeatherForecast>> Handle(WeatherForecastRecordRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}
