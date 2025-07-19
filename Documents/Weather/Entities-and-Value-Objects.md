# Entites and Value Objects

The domain *Weather Forecast* object is defined as:

```csharp
public sealed record DmoWeatherForecast
{
    public WeatherForecastId Id { get; init; } = new(Guid.Empty);
    public Date Date { get; init; }
    public Temperature Temperature { get; init; }
    public string Summary { get; init; } = "Not Defined";
}
```

1. It's a record: immutable by default.  All the properties are `init`: can only be set during object construction.
1. It's sealed: cannot be inherited.
1. `Id`, `Date` and `Temperature` are all value objects: no primitive obsession.
1. Default values are set where appropriate.

##  Strongly Typed Ids

Why? see this set of articles - [Using strongly typed entity Ids to avoid primitive obsession](https://andrewlock.net/series/using-strongly-typed-entity-ids-to-avoid-primitive-obsession/)

The `WeatherForecastId` is a strongly typed id.  It's a value object that wraps a `Guid`.  It's defined like this:

```csharp
public readonly record struct WeatherForecastId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static WeatherForecastId Create => new(Guid.CreateVersion7());
    public static WeatherForecastId Default => new(Guid.Empty);

    public override string ToString()
    {
        return this.IsDefault ? Value.ToString() : "Not Valid";
    }
}
```

Note that `Create` creates a new sequential Guid that is database index friendly. 

The `IEntityId` provides necessary functionality for generic objects to work with the id further down the data pipeline.

```csharp
public interface IEntityId
{
    public bool IsDefault { get; }
}
```

## Value Objects

Date and Temperature are value objects.

Date is a little fat to deal with various constructors.

```csharp
public readonly record struct Date
{
    public DateOnly Value { get; init; }
    public bool IsValid { get; init; }

    public DateTime ToDateTime => this.Value.ToDateTime(TimeOnly.MinValue);

    public Date() 
    {
        this.Value = DateOnly.MinValue;
        this.IsValid = false;
    }

    public Date(DateOnly date)
    {
        this.Value = date;
        if (date > DateOnly.MinValue)
            this.IsValid = true;
    }

    public Date(DateTime date)
    {
        this.Value = DateOnly.FromDateTime(date);
        if (date > DateTime.MinValue)
            this.IsValid = true;
    }

    public Date(DateTimeOffset date)
    {
        this.Value = DateOnly.FromDateTime(date.DateTime);
        if (date > DateTime.MinValue)
            this.IsValid = true;
    }

    public override string ToString()
    {
        return this.IsValid ? this.Value.ToString("dd-MMM-yy")  : "Not Valid";
    }
}
```

And Temperature:

```csharp
public readonly record struct Temperature
{
    public decimal TemperatureC { get; init; } = -273;
    public bool IsValid { get; init; }
    [JsonIgnore] public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);

    public Temperature() { }

    /// <summary>
    /// temperature should be provided in degrees Celcius
    /// </summary>
    /// <param name="temperature"></param>
    public Temperature(decimal temperatureAsDegCelcius)
    {
        this.TemperatureC = temperatureAsDegCelcius;
        if (temperatureAsDegCelcius > -273)
            IsValid = true;
    }

    public override string ToString()
    {
        return this.IsValid ? TemperatureC.ToString() : "Not Valid";
    }
}
```

## Providers

There are two providers associated with each domain entity.

They are defined as interfaces:

1. `INewRecordProvider<TRecord>` is a DI registered service to provide new copies of `TRecord`.
1. `IRecordIdProvider<TKey>` provides methods to covert keys to and from objects.


#### INewRecordProvider

The interface:

```csharp
public interface INewRecordProvider<TRecord> where TRecord : new()
{
    public TRecord NewRecord();
    public TRecord DefaultRecord();
}
```

And the implementation for `DmoWeatherForecast`:

```csharp
public class NewWeatherForecastProvider : INewRecordProvider<DmoWeatherForecast>
{
    public DmoWeatherForecast NewRecord()
    {
        return new DmoWeatherForecast()
        {
            Id = WeatherForecastId.Create,
            Date = new(DateTime.Now.AddDays(1))
        };
    }

    public DmoWeatherForecast DefaultRecord()
    {
        return new DmoWeatherForecast { Id = WeatherForecastId.Default };
    }
}
```

Note that the `NewRecord` method creates a new `WeatherForecast` with a new `Id` and a `Date` set to tomorrow, while the `DefaultRecord` method creates a new `WeatherForecast` with default values.
 

#### IRecordIdProvider

The interface:
```csharp
public interface IRecordIdProvider<TKey>
{
    public TKey GetKey(object key);
    public object GetValueObject(TKey key);
}
```

And the implementation for `WeatherForecastId`:

```csharp
public class WeatherForecastIdProvider : IRecordIdProvider<WeatherForecastId>
{
    public WeatherForecastId GetKey(object key)
    {
        if (key is Guid value)
            return new(value);

        throw new InvalidKeyProviderException("Object provided is not a WeatherForecastId Value");
    }
    public object GetValueObject(WeatherForecastId key)
    {
        return key.Value;
    }
}
```

Which is registered in the DI container:

```csharp
services.AddSingleton<IRecordIdProvider<WeatherForecastId>, WeatherForecastIdProvider>();
```
