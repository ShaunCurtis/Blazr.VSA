# Notifications

The application uses a simple *Message Bus* implementation to notify interested parties of data changes.

The *Blazr.Gallium* package/library provides the base functionality.

The service is registered:

```csharp
services.AddScoped<IMessageBus, MessageBus>();
```

Data entities changes are notified in the *Mediatr* command handlers.  The `WeatherForecastCommandHandler` is an example of a command handler that publishes a message to the message bus.

```csharp
public record WeatherForecastCommandHandler : IRequestHandler<WeatherForecastCommandRequest, CommandResult<WeatherForecastId>>
{
    private ICommandHandler _handler;
    private IMessageBus _messageBus;

    public WeatherForecastCommandHandler(ICommandHandler handler, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _handler = handler;
    }

    public async Task<CommandResult<WeatherForecastId>> Handle(WeatherForecastCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _handler.ExecuteAsync<DboWeatherForecast>(new CommandRequest<DboWeatherForecast>(
            Item: DboWeatherForecastMap.Map(request.Item),
            State: request.State,
            Cancellation: cancellationToken
        ));

        if (result.KeyValue is DboWeatherForecast record)
        {
            _messageBus.Publish<DmoWeatherForecast>(DboWeatherForecastMap.Map(record));
            return CommandResult<WeatherForecastId>.SuccessWithKey(new WeatherForecastId(record.ID));
        }

        return CommandResult<WeatherForecastId>.Failure(new CommandException($"Returned object was not a DmoWeatherForecast"));
    }
}
```

And `GridFormBase` consumes it:

```csharp
    protected async override Task OnInitializedAsync()
    {
        this.MessageBus.Subscribe<TRecord>(this.OnStateChanged);
        //....
    }
    private void OnStateChanged(object? message)
    {
        this.InvokeAsync(quickGrid.RefreshDataAsync);
    }

    public void Dispose()
    {
        this.MessageBus.UnSubscribe<TRecord>(this.OnStateChanged);
    }
```