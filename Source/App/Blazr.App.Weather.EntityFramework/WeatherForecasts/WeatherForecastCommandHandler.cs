/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.EntityFramework;

/// <summary>
/// Mediatr Handler for executing commands against a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed record WeatherForecastCommandHandler : IRequestHandler<WeatherForecastCommandRequest, Result<WeatherForecastId>>
{
    private readonly IMessageBus _messageBus;
    private readonly IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastCommandHandler(IDbContextFactory<InMemoryWeatherTestDbContext> factory, IMessageBus messageBus)
    {
        _factory = factory;
        _messageBus = messageBus;
    }

    public async Task<Result<WeatherForecastId>> Handle(WeatherForecastCommandRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        var result = await dbContext.ExecuteCommandAsync<DboWeatherForecast>(new CommandRequest<DboWeatherForecast>(
            Item: WeatherForecastMap.Map(request.Item),
            State: request.State
        ), cancellationToken);

        if (result.HasNotSucceeded(out DboWeatherForecast? record))
            return result.ConvertFail<WeatherForecastId>();

        var id = new WeatherForecastId(record.WeatherForecastID);

        _messageBus.Publish<DmoWeatherForecast>(id);

        return Result<WeatherForecastId>.Success(id);
    }
}
