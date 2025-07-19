# Entities

Entities are business objects that encapsulate the data, functionality and rules applicable to the object.

They may be complex, such as a invoice and it's invoice items , or simple, such as a weather forecast.

In this first discussion I'll deal with the simple Weather Forecast.

Come on [you say], surely a simple weather forecast can be represented as a single class such as:

```csharp
public class WeatherForecast
{
    [Key] public Guid WeatherForecastID { get; set; }
    public DateTime Date { get; set; }
    public decimal TemperatureC { get; set; }
    public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);
  public string? Summary { get; set; }
}
```
Of course it can, but consider how open this is.  How easy it is to write buggy code, create meaningless weather forecasts, use values that are out of range.

The above class exhibits a behaviour known as *Primitive Obsession*.  Basically representing a concept with a primitive data type.

Consider:

1. The Temperature.  Is `decimal.MinValue` a valid temperature?  No, anything below -273 [if out base unit is celcius] is invalid.

2. TemperatureF. Why build display formatting into the object?

3. The Date.  This is a daily weather forecast.  Why do we need a time?  It makes equality checking difficult.  Should we be able to add a new weather forecast in the past?

4. The Id.  If we had a `public Guid WeatherStationID { get; set; }`, what would stop us passing a `WeatherStationID` into a method to get a weather forecast by id?

5. What's to stop us inadvertently modifying a `WeatherForecast` object when we didn't mean to?

Our object screams database design, not domain/business layer design.  In our desire to keep things simple, we shy away from defining multiple objects and instead create a complex object, that is like a sieve: riddled with holes.

Very few consider how many ways an object they create can be misused and abused?

Lets reconsider our object [the owner properties are part of the Authentication/Authorization demo]:

```csharp
public sealed record DmoWeatherForecast
{
    public WeatherForecastId Id { get; init; } = new(Guid.Empty);
    //public IdentityId OwnerId { get; init; } = new(Guid.Empty);
    //public string Owner { get; init; } = string.Empty;
    public Date Date { get; init; }
    public Temperature Temperature { get; init; }
    public string Summary { get; init; } = "Not Defined";
}
```

We now have a strongly typed Id value object with a default value:


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

A value object to represent our date with various constructors and a default `ToString` format:

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

And a `Temperature` value object for temperature, with validation and unit convertion built in:

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

Note that they are all have value semantics and are immutable: declared as `readonly record struct`.  They also carry out basic validation and have a validity flag where appropriate.

## Dmo, Dsr, Dvo and Dbo Objects

*Dmo* objects are **Domain Objects**.

*Dsr* are **Data State Records** that consist of a *Dmo* wuth it's current state.  *Drs* objects are used in Aggregate Composite objects to track individual record state.

*Dvo* objects are **Data View Objects** used in the infrastructure layer to retrieve data from the data store.

*Dbo* objects are **DataBase Objects** used to apply commands to the data store.
 
