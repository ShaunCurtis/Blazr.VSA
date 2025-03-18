/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Gallium;

namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing commands against a WeatherForecast Entity
/// </summary>
public sealed record WeatherForecastCommandHandler : IRequestHandler<WeatherForecastCommandRequest, Result<WeatherForecastId>>
{
    private ICommandBroker _broker;
    private IMessageBus _messageBus;

    public WeatherForecastCommandHandler(ICommandBroker broker, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _broker = broker;
    }

    public async Task<Result<WeatherForecastId>> Handle(WeatherForecastCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _broker.ExecuteAsync<DboWeatherForecast>(new CommandRequest<DboWeatherForecast>(
            Item: WeatherForecastMap.Map(request.Item),
            State: request.State
        ), cancellationToken);

        if (!result.HasSucceeded(out DboWeatherForecast? record))
            return result.ConvertFail<WeatherForecastId>();

        var id = new WeatherForecastId(record.WeatherForecastID);

        _messageBus.Publish<DmoWeatherForecast>(id);

        return Result<WeatherForecastId>.Success(id);
    }
}
