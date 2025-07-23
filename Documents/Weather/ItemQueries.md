# Data Pipeline Item Queries

Item Queries retrieve a single item from the data store based on an identity key.

We can emulate what the Ui does in an XUnit test.

The initial setup gets the DI ServiceProvider and the EntityProvider for the `DmoWeatherForecast` entity.  It uses a demo InMemory database loaded from the `WeatherTestDataProvider`. 

```csharp
[Fact]
public async Task GetAForecast()
{
    var provider = GetServiceProvider();

    var entityProvider = provider.GetRequiredService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    var uIEntityProvider = provider.GetRequiredService<IUIEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
```
 It gets a random record from the TestDataProvider and sets up the control values.
```csharp
    var testItem = _testDataProvider.WeatherForecasts.Skip(Random.Shared.Next(50)).First();
    var testRecord = this.AsDmoWeatherForecast(testItem);
    var testId = testRecord.Id;
```

Set up the outputs that we'll test:

```csharp
    bool result = false;
```
Get the Read UI Broker for the Id. 

```csharp
    var uiBroker = await uIEntityProvider.GetReadUIBrokerAsync(controlId);
```
Finally get the result and test:

```csharp
    uiBroker.LastResult.Output(
        success: () => result = true,
        failure: (ex) => result = false);

    // check the query was successful
    Assert.True(result);
    // check it matches the test record
    Assert.Equal(controlRecord, uiBroker.Item);
}
```

## UIEntityProvider

The UIEntityProvider provides a set of entity specific factory methods, services and data.

We get the `IReadUIBroker` from `DmoWeatherForecast` from the `WeatherForecastUIEntityProvider`.

`GetReadUIBrokerAsync` is the factory method that creates the broker in the DI context and loads it ready to use.

```csharp
    public async ValueTask<IReadUIBroker<DmoWeatherForecast, WeatherForecastId>> GetReadUIBrokerAsync(WeatherForecastId id)
    {
        var presenter = ActivatorUtilities.CreateInstance<ReadUIBroker<DmoWeatherForecast, WeatherForecastId>>(_serviceProvider);
        await presenter.LoadAsync(id);

        return presenter;
    } 
```

Note we use `ActivatorUtilities` to create the object in the context of the DI Service container.  The `ReadUIBroker` isn't a DI service.  It's scoped to the display form.

## ReadUIBroker

The `ReadUIBroker` manages the data for the UI display form.

It has two readonly properties to provide access to the record and any error messages, and an event raised when the record changes.

```csharp
public TRecord Item { get; protected set; } = new TRecord();
public event EventHandler? RecordChanged;
public Result LastResult { get; protected set; } = Result.Success();
```

It's single public method calls thr internal `GetRecordItemAsync` and assigns the returned result to `LastResult`. 

```csharp
public async ValueTask LoadAsync(TKey id)
    => LastResult = await GetRecordItemAsync(id);
```

The internal `GetRecordItemAsync` method does the work.  It's written in functional style.

1. It gets the Result object for id.
2. Checks if the id is a default value and switches the result to failure mode if it is.
3. It saves the id internally in the object.
4. It executes the async entity specific request function provided by the EntityProvider.
5. On success, it saves the record to the object.
6. Returns a `Result`. 

```csharp
private async Task<Result> GetRecordItemAsync(TKey id)
    => await Result<TKey>.Create(id)
        .ApplyTransformOnException(id.IsDefault, "The record Id is default.  Mo record retrieved.")
        .ApplySideEffect((recordId) => _key = recordId)
        .ApplyTransformOnException(_entityProvider.RecordRequestAsync)
        .TaskSideEffectAsync(success: (record) => this.Item)
        .MapTaskToResultAsync();
```

## WeatherEntityProvider

`RecordRequestAsync` is a `Func` delegate that returns:

 -  `NewRecordRequestAsync` - if `id` is the default value
 -  `ExistingRecordRequestAsync` - if `id` is a valid value

```csharp
public Func<WeatherForecastId, Task<Result<DmoWeatherForecast>>> RecordRequestAsync
    => (id) => id.IsDefault ? NewRecordRequestAsync(id) : ExistingRecordRequestAsync(id);
```

Note that the code uses a default `Guid` to signify a request for a new record.   

