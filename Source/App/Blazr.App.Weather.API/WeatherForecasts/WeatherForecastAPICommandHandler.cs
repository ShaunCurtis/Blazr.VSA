/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.API;

/// <summary>
/// Mediatr Handler for executing commands against a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed record WeatherForecastAPICommandHandler : IRequestHandler<WeatherForecastCommandRequest, Result<WeatherForecastId>>
{
    private readonly IMessageBus _messageBus;
    private IHttpClientFactory _factory;

    public WeatherForecastAPICommandHandler(IHttpClientFactory clientFactory, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _factory = clientFactory;
    }

    public async Task<Result<WeatherForecastId>> Handle(WeatherForecastCommandRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}
