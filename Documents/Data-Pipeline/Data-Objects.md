#  Data Objects

The application data pipeline is designed on *Clean Design* principles.

Core business objects are defined as entities using value objects where appropriate.

All data is *READONLY*: either `record` or `readonly record struct`.  Data retrieved from a data source is a **copy** of the data within the data source.  Data is mutated by creating a new copy, not by changing the copy.

The Weather Forecast entity provides a good example.

## Entity

The core entity is defined as:

```csharp
public sealed record DmoWeatherForecast : ICommandEntity
{
    public WeatherForecastId Id { get; init; } = new(Guid.Empty);
    public IdentityId OwnerId { get; init; } = new(Guid.Empty);
    public string Owner { get; init; } = string.Empty;
    public Date Date { get; init; }
    public Temperature Temperature { get; init; }
    public string Summary { get; init; } = "Not Defined";
}
```

Note:

1. The object is sealed - there's no business case for inheritance.
2. It's a record and all the properties are `init` - it's immutable.
3. The use of a strongly typed Id based on a Guid.
4. Value objects for the Date and Temperature.
5. Foreign key strongly typed Id for the owner.


```csharp
public readonly record struct WeatherForecastId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static WeatherForecastId Default => new(Guid.Empty);

    public override string ToString()
    {
        return Value.ToString();
    }
}
```

`Temperature` encapsulates the temperature entity and provides the various unit versions and validation.

```csharp
public readonly record struct Temperature
{
    public decimal TemperatureC { get; init; } = -273;
    public bool IsValid { get; private init; }
    public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);

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

`Date` provides validation and string formatting.

```csharp
public readonly record struct Date
{
    public DateOnly Value { get; init; }
     public bool IsValid { get; private init; }

    public Date() { }

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

## Database Infrastructure Objects

Entities normal need converting to save and retrieve from data stores.  As we're using a SQL database and records with foreign keys that need views to produce the data for the entity we have a `DboWeatherForecast` database table object that used for commands.  All data queries return a view based `DvoWeatherForecast` object.

Note properties are now primitives and we have the possiblity of nulls.

The database table record to match the database table is:

```csharp
public sealed record DboWeatherForecast : ICommandEntity
{
    [Key] public Guid WeatherForecastID { get; init; } = Guid.Empty;
    public Guid OwnerID { get; init; } = Guid.Empty;
    public DateTime Date { get; init; }
    public decimal Temperature { get; init; }
    public string? Summary { get; init; }
}
```

The view object:

```csharp
public sealed record DvoWeatherForecast
{
    [Key] public Guid WeatherForecastID { get; init; } = Guid.Empty;
    public Guid OwnerID { get; init; } = Guid.Empty;
    public string Owner { get; set; } = "[Not Set]";
    public DateTime Date { get; init; }
    public decimal Temperature { get; set; }
    public string? Summary { get; set; }
}
```

## Mapping

We can define a mapper that maps to the Dmo entity from the Dvo object and from the Dmo entity to the Dbo object. 

```csharp
public sealed class WeatherForecastMap 
{
    public static DmoWeatherForecast Map(DvoWeatherForecast item)
        => new()
        {
            Id = new(item.WeatherForecastID),
            Date = new(item.Date),
            OwnerId = new(item.OwnerID),
            Owner = item.Owner ?? "Not Defined",
            Temperature = new(item.Temperature),
            Summary = item.Summary ?? "Not Defined"
        };

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            WeatherForecastID = item.Id.Value,
            OwnerID = item.OwnerId.Value,
            Date = item.Date.Value.ToDateTime(TimeOnly.MinValue),
            Temperature = item.Temperature.TemperatureC,
            Summary = item.Summary
        };
}
```

## Editing

All our objects are records so we need to create an edit context for the UI editor.

1. `BaseRecordEditContext` contains the boilerplate code and is shown below.
2. The `[TrackState]` attribute tells the `EditStateTracker` component to track this property's state in the `EditForm`.

```csharp
public sealed class WeatherForecastEditContext : BaseRecordEditContext<DmoWeatherForecast, WeatherForecastId>, IRecordEditContext<DmoWeatherForecast>
{
    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public override DmoWeatherForecast AsRecord =>
        this.BaseRecord with
        {
            Date = new(this.Date ?? DateTime.Now),
            Summary = this.Summary ?? "Not Set",
            Temperature = new(this.Temperature)
        };
    public WeatherForecastEditContext() : base() { }

    public WeatherForecastEditContext(DmoWeatherForecast record) : base(record) { }

    public override IDataResult Load(DmoWeatherForecast record)
    {
        if (!this.BaseRecord.Id.IsDefault)
            return DataResult.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;
        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.Value.ToDateTime(TimeOnly.MinValue);

        return DataResult.Success();
    }
}
```

The `BaseRecordEditContext` boilerplate code:

```csharp
public abstract class BaseRecordEditContext<TRecord, TKey>
    where TRecord : class, new()
{
    public TRecord BaseRecord { get; protected set; } = new();

    public abstract TRecord AsRecord { get; }

    public bool IsDirty => this.BaseRecord != this.AsRecord;

    public BaseRecordEditContext()
    {
        this.Load(this.BaseRecord);
    }

    public BaseRecordEditContext(TRecord record)
    {
        this.Load(record);
    }

    public abstract IDataResult Load(TRecord record);

    public void Reset()
    {
        var record = this.BaseRecord;
        this.BaseRecord = new();
        this.Load(record);
    }

    public void SetAsPersisted()
    {
        var record = this.AsRecord;
        this.BaseRecord = new();
        this.Load(record);
    }
}
```

## Edit Context Validation

The application uses *Fluent Validation*.  The validator runs against the `WeatherForecastEditContext`.

```csharp
public class WeatherForecastEditContextValidator : AbstractValidator<WeatherForecastEditContext>
{
    public WeatherForecastEditContextValidator()
    {
        this.RuleFor(p => p.Summary)
            .MinimumLength(3)
            .WithState(p => p);

        this.RuleFor(p => p.Date)
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("Date must be in the future")
            .WithState(p => p);

        this.RuleFor(p => p.Temperature)
            .GreaterThanOrEqualTo(-60)
            .LessThanOrEqualTo(70)
            .WithState(p => p);
    }
}
```

## Newing up Records

It's easy to new up a record like this `new()`, but it doesn't always get you what you want.

 `IEntityProvider<TRecord>` abstracts the creation in a DI defined service.

 ```csharp
     public TRecord NewRecord { get; }
```

Here's the `WeatherForecastEntityProvider`.  It's registered as a *Scoped* service, so can `SetOwnerIdContext` can be used to set the context for the SPA. 

```csharp
public class WeatherForecastEntityProvider : IEntityProvider<DmoWeatherForecast, WeatherForecastId>
{
    private IdentityId _ownerId = IdentityId.Default;

    //.....


    public void SetOwnerIdContext(IdentityId ownerId)
    {
        _ownerId = ownerId;
    }

    public DmoWeatherForecast NewRecord
        => new DmoWeatherForecast { Id = WeatherForecastId.Default, OwnerId = _ownerId };
}
```
