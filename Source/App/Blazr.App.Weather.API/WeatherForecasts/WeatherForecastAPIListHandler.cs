/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.EntityFramework;

/// <summary>
/// Mediatr Handler for executing list requests against a WeatherForecast Entity in a Entity Framework Context
/// </summary>
public sealed class WeatherForecastAPIListHandler : IRequestHandler<WeatherForecastListRequest, Result<ListItemsProvider<DmoWeatherForecast>>>
{
    private IHttpClientFactory _factory;

    public WeatherForecastAPIListHandler(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<ListItemsProvider<DmoWeatherForecast>>> Handle(WeatherForecastListRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}
