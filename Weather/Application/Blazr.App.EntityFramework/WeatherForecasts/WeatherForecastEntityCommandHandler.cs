/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing commands against a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed record WeatherForecastEntityCommandHandler : IRequestHandler<WeatherForecastEntityCommandRequest, Result<WeatherForecastEntity>>
{
    private readonly IMessageBus _messageBus;
    private readonly IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastEntityCommandHandler(IDbContextFactory<InMemoryWeatherTestDbContext> factory, IMessageBus messageBus)
    {
        _factory = factory;
        _messageBus = messageBus;
    }

    public async Task<Result<WeatherForecastEntity>> HandleAsync(WeatherForecastEntityCommandRequest request, CancellationToken cancellationToken)
    {
        var commandRequest = new CommandRequest<DboWeatherForecast>(
                Item: DboWeatherForecast.Map(request.Item.WeatherForecastRecord.Record),
                State: request.Item.WeatherForecastRecord.State);

        // update the record in the data store
        var result = await _factory.CreateDbContext()
            .ExecuteCommandAsync<DboWeatherForecast>(commandRequest, cancellationToken)
            .ExecuteActionOnSuccessAsync(record => _messageBus.Publish<DmoWeatherForecast>(new WeatherForecastId(record.WeatherForecastID)));

        return result.ToResult<WeatherForecastEntity>(request.Item);
    }
}
