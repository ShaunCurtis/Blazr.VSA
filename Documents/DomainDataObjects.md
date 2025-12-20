# Domain Data Objects

The domain data objects (DMOs) are the core data objects used in a functional programming style application.  Before delving into functional programming, you need to understand some fundimental differences from the data objects you've probably familair with in OOP.

## Immutability

If you already use immutable objects in OOP, you can gloss over this section.  The chances are though, you aren't.

Consider the `Invoice` record used in the DotNet templates.

```csharp
public class DmoInvoice
{
    public Guid Id { get; set; }
    public Guid Customer { get; set; }
    public Decimal TotalAmount { get; set; }
    public DateTime Date { get; set; }
}
```

This is mutable.  There are some major problems with this behaviour:

1. Anyone can change it: It's full of side effects.
1. You have to write a custom comparitor to chack if two records are equal.
1. It's full of primitive types that don't represent real world things.

### The new DmoInvoice 

```csharp
public sealed record DmoInvoice
{
    public InvoiceId Id { get; init; } = InvoiceId.Default;
    public FkoCustomer Customer { get; init; } =  FkoCustomer.Default;
    public Money TotalAmount { get; init; } = Money.Default;
    public Date Date { get; init; }

    public static DmoInvoice CreateNew()
        => new() { Id = InvoiceId.Create, Date = new(DateTime.Now) };
}
```

It's immutable and sealed: there's no valid reason to inherit from this object.  Everything is now a value object that represents a real world thing.

## Primitive Obsession

Is `MyColdestDay.TemperatureC = -10000000` a valid temperature?  To protect against such assignments, we write defensive validation code whenever we generate a `WeatherForecast`.  The solution to this problem is to create a value object that represents temperature and only contains a valid entry.

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

In `DmoInvoice` everything is a value object.

#### InvoiceId

This is still based on a `Guid` but it represents a real world thing: an invoice identifier.  You can't accidentally assign a `Guid` to an `InvoiceId`, or search for an invoive with a customer Id.  You'll also notice custom methods for creating new Ids, checking for default values and string output.

```csharp
public readonly record struct InvoiceId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static InvoiceId Create => new(Guid.CreateVersion7());
    public static InvoiceId Default => new(Guid.Empty);

    public override string ToString()
        => this.IsDefault ? "Default" : Value.ToString();

    public string ToString(bool shortform)
        => this.IsDefault ? "Default" : Value.ToString().Substring(28);
}
```

#### Date

Follows a similar pattern:

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
### Money

As does money whih also adds 

```csharp
public readonly record struct Money
{
    public decimal Value { get; private init; }
    public bool HasValue { get; private init; }

    public bool IsZero => Value != 0;

    public static Money Default => new(0);

    public Money(decimal value)
    {
        if (value >= 0)
        {
            Value = value;
            HasValue = true;
            return;
        }
        Value = 0;
        HasValue = false;
    }

    public override string ToString()
    {
        if (this.HasValue)
            return Value.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"));

        return "Not Set";
    }

    public static Money operator +(Money one, Money two)
        => new Money(one.Value + two.Value);

    public static Money operator -(Money one, Money two)
        => new Money(one.Value - two.Value);

    public static Money operator *(Money one, Money two)
        => new Money(one.Value * two.Value);

    public static Money operator /(Money one, Money two)
        => new Money(Decimal.Round(one.Value / two.Value, 2, MidpointRounding.AwayFromZero));
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
