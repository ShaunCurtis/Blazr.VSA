# Domain Data Objects

Before delving into functional programming, you need to understand some fundimental differences from the data objects you've probably familair with in OOP.

## Immutability

If you already use immutable objects in OOP, you can gloss over this section.  The chances are thougfh you aren't.

Consider the `WeatherForecast` record used in the DotNet templates.

```csharp
public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

This is mutable.  There are two major problems with this behaviour:

1. Anyone can change it: it's open to buggy behaviour.
2. You have to write a custom comparitor to chack if two records are equal.

### Step 1 

is to turn it into a record:

```csharp
public record WeatherForecast
{
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

## Value Objects and Identity

`WeatherForecast` is currently a value object.

Consider this code.

```csharp
var date = DateOnly.FromDateTime(DateTime.Now);

var record1 = new WeatherForecast { Date = date, TemperatureC = 2, Summary = "Cold" };
var record2 = new WeatherForecast { Date = date, TemperatureC = 2, Summary = "Cold" };

Console.WriteLine(record1.Equals(record2));
```

If `WeatherForecast` is a `class` the result is `false` because we have two objects with different references.

On the other hand, if `WeatherForecast` is a `record` the result is `true` because the two objects contain identical data.

They are both fundimentally still reference objects.  The difference is in compilation.  The compiler builds a different set of comparitors for a `record`.

The `WeatherForecast` needs an identity.  Here we use a Guid.

```csharp
public record WeatherForecast
{
    public Guid Id {get; init;}
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

## Primitive Obsession

Is `MyColdestDay.TemperatureC = -10000000` a valid temperature?  To protect against such assignments we have to write defensive validation code whenever we generate a `WeatherForecast`.  This illustrates the problem in using primitive types to represent real world things.

The solution to this problem is to create a value object that represents temperature and only contains a valid entry.

Here's one implementation that throws an exception if the decimal is out of range.  

```csharp
public readonly record struct Temperature
{
    public decimal TemperatureC { get; init; } = -273;
    public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);

    public Temperature() { }

    /// <summary>
    /// temperature should be provided in degrees Celcius
    /// </summary>
    /// <param name="temperature"></param>
    public Temperature(decimal temperatureAsDegCelcius)
    {
        if (temperatureAsDegCelcius < -273)
            throw new ArgumentOutOfRangeException("The temperature is outside thw valid range ( -273 to decimal.Max)");

        this.TemperatureC = temperatureAsDegCelcius;
    }

    public override string ToString()
        => $"{TemperatureC.ToString()} degC";
}
```

And a slightly different one that takes a more functional approach.

```csharp
public readonly record struct Temperature
{
    public decimal TemperatureC { get; init; } = -273;
    public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);
    public bool IsValid { get; private init; } = false;
    public string? Error { get; private init; }

    public Temperature() { }

    /// <summary>
    /// temperature should be provided in degrees Celcius
    /// </summary>
    /// <param name="temperature"></param>
    public Temperature(decimal temperatureAsDegCelcius)
    {
        if (temperatureAsDegCelcius < -273)
            this.Error = "The temperature is outside thw valid range ( -273 to decimal.Max)";

        this.IsValid = true;
        this.TemperatureC = temperatureAsDegCelcius;
    }

    public override string ToString()
        => this.IsValid ? TemperatureC.ToString() : "Not Valid";
}
```

Date and Id are simiarly value objects.

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
        => this.IsValid ? this.Value.ToString("dd-MMM-yy")  : "Not Valid";
}
```

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

Some notes on these objects:

1. All of these objects are `readonly record struct`.  They are value types with value sematics.
2. They are all immutable, like `int` and `bool`.
3. 

## Our new Domain Weather Forecast

The final domain object looks like this. `Summary` is still a string because that's what it is.  Note the record is `sealed`: there's no valid reason to inherit from this object.

```csharp
public sealed record DmoWeatherForecast : ICommandEntity
{
    public WeatherForecastId Id { get; init; } = new(Guid.Empty);
    public Date Date { get; init; }
    public Temperature Temperature { get; init; }
    public string Summary { get; init; } = "Not Defined";
}
```
