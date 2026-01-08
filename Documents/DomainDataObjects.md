# Domain Data Objects

The domain data objects (DMOs) are the core data objects used in a functional programming style application.  Domain data objects implement three design principles:

1. Immutability
2. Validity
3. Unique Identity

Consider this imperitive style data object representing a customer:

```csharp
public class Customer
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}
```

It makes life simple because you can use the same object to get the data from the database an edit it in the UI.  

Now consider the downside:

1. Anyone can change it: It's full of side effects.
1. You have to write a custom comparitor to chack if two records are equal.
1. It's has primitive types that don't represent real world things.

Even though it's extemely basic consider it's state.  Is `Guid.Empty` a valid state for CustomerId? You need to write defensive code all over the place to constantly check it's validity. 

### The New Customer 

First somw value objects.

`CustomerId` looks like this.

1. It's a `readonly record struct`.
1. Creation is controlled: youcan't just new up an instance.
1. It has a mechanism for differentiating between a new and an existing id
1. It has a custom comparator that only compares the Guid.
1. It has a custom `ToString()` to display the Id.

```csharp
public readonly record struct CustomerId : IEquatable<CustomerId>
{
    public Guid Value { get; private init; }
    public bool IsNew { get; private init; }

    private CustomerId(Guid value)
        => Value = value;

    public CustomerId()
    {
        Value = Guid.CreateVersion7();
        IsNew = true;
    }

    public static CustomerId Load(Guid id)
        => id == Guid.Empty
            ? throw new InvalidGuidIdException()
            : new CustomerId(id);

    public static CustomerId NewId => new() { IsNew = true };

    public override string ToString()
        => Value.ToString();

    public string ToString(bool shortform)
        => Value.ToString().Substring(28);

    public bool Equals(CustomerId other)
        => this.Value == other.Value;

    public override int GetHashCode()
        => HashCode.Combine(this.Value);
}
```

`Title` presents the standard `string` problems.  In this case:

1. The field is restricted to 100 characters, and automatically handle anything greater.
2. There's a default value if nothing is entered.
1. There's a custom `ToString()`. 

```csharp
public readonly record struct Title
{
    public string Value { get; private init; }
    public static readonly string DefaultValue = "[NO TITLE SET]";
    public bool IsDefault => this.Value.Equals(DefaultValue);

    public Title(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            this.Value = DefaultValue;
            return;
        }

        if (name.Length > 100)
        {
            this.Value = string.Concat(name.Substring(98), "..") ;
            return;
        }

        this.Value = name.Trim();
    }

    public static Title Default => new() { Value = DefaultValue};

    public override string ToString()
        => Value.ToString();
}
```

Anf finally `DmoCustomer`

```csharp
public sealed record DmoCustomer : ICommandEntity
{
    public CustomerId Id { get; init; }
    public Title Name { get; init; }

    public static DmoCustomer NewCustomer()
        => new DmoCustomer() { Id = CustomerId.NewId };
}
```

1. It's immutable and sealed: there's no valid reason to inherit from this object.  1. Everything is now a value object that represents a real world thing.
1. It has constrolled state.  No more defensive code, or bugs caused by invalid state.   

## Primitive Obsession

**Primitive Obsession** is the use of primitive objects, such as `int`, `decimal` or `string` to represent domain concepts such as record Ids, Titles, Money and Temperature.  Temperature is a good example to consider. 

Is `MyColdestDay.TemperatureC = -10000000` a valid temperature?

Using a primite means we need to write defensive code to guard against assigning invalid temperatures.

The solution is to create a value object that represents temperature and only contains a valid entry.

This implementation that throws an exception if the decimal is out of range.  

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
            throw new ArgumentOutOfRangeException("The temperature is outside the valid range ( -273 to decimal.Max)");

        this.TemperatureC = temperatureAsDegCelcius;
    }

    public override string ToString()
        => $"{TemperatureC.ToString()} degC";
}
```

## The Data Pipeline

*Core* domain code has no dependencies with I/O.  No Entity Framework attributes, no Dapper annotations, no UI stuff, nothing.

In the demo, EF Core is restricted to the *Infrastructure* domain.  The application implements CQS so has different query and command pipelines.

Query objects are *Data View Objects* - *Dvo* which implement a `Map` function builds a `DmoInvoice` object from the *Dvo*.  *Dvo* objects use data types that match the data store objects. 

```csharp
public sealed record DvoCustomer
{
    [Key] public Guid CustomerID { get; init; } = Guid.Empty;
    public string? CustomerName { get; set; }

    public static DmoCustomer Map(DvoCustomer item)
        => new()
        {
            Id = CustomerId.Load(item.CustomerID),
            Name = new (item.CustomerName ?? Title.DefaultValue)
        };
}
```

Next the Domain to Data Store object *Dbo* is a *Database Object*.

```csharp
public sealed record DboInvoice 
{
    [Key] public Guid InvoiceID { get; init; }
    public Guid CustomerID { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime Date { get; init; }

    public static DboInvoice Map(DmoInvoice item)
    => new()
    {
        InvoiceID = item.Id.Value,
        CustomerID = item.Customer.Id.Value,
        TotalAmount = item.TotalAmount.Value,
        Date = item.Date.ToDateTime
    };
}
```
