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

It appears to make life simple: the same object to get the data from the database, display and edit it in the UI.  

Now consider the downside:

1. Anyone can change it: It's full of side effects.
1. You have to write a custom comparitor to chack if two records are equal.
1. It's has primitive types that don't represent real world things.
1. The compiler is *blind*.  It doesn't know your code ia submitting an Invoice `Id` as a customer `Id`.

Even though it's extemely basic consider state.  Is `Guid.Empty` a valid state for CustomerId? You need to write defensive code all over the place to constantly check it's validity. 

> Forget restricting the number of data objects is good practice.  It isn't.

### The New Customer 

First some value objects.

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

Query objects are *Data View Objects* - *Dvo*s implement a `Map` function which build a `DmoInvoice` object from the *Dvo*.  *Dvo* objects use data types that match the data store objects. 

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

Command objects are *Database Objects* - *Dbo*.  They implement a static `Map` function to build a `Dbo` from a `Dmo`.

```csharp
public sealed record DboCustomer
{
    [Key] public Guid CustomerID { get; init; } = Guid.Empty;
    public string CustomerName { get; init; } = string.Empty;

    public static DboCustomer Map(DmoCustomer item)
        => new()
        {
            CustomerID = item.Id.Value,
            CustomerName = item.Name.Value
        };
}
```

## The UI

Mutating a simple domain object is as easy as:

```csharp
var mutatedCustomer = OldCustomer with { CustomerName = "Updated Name" };
```

However, in an edit form you need to provide a editable properties.  My framework uses what I've named the *Mutor Pattern*.

First an interface:

```csharp
public interface IRecordMutor<TRecord>
    where TRecord : class
{
    public TRecord BaseRecord { get; }
    public bool IsDirty { get; }
    public bool IsNew { get; }
    public TRecord Record { get; }
    public void Reset();
    public RecordState State { get; }
}
```

And an abstract base class:

```csharp
public abstract class RecordMutor<TRecord>
    where TRecord : class
{
    public TRecord BaseRecord { get; protected set; } = default!;
    public bool IsDirty => !this.Record.Equals(BaseRecord);
    public virtual bool IsNew { get; }
    public virtual TRecord Record { get; } = default!;

    public RecordState State => (this.IsNew, this.IsDirty) switch
    {
        (true, _) => RecordState.NewState,
        (false, false) =>RecordState.CleanState,
        (false, true) => RecordState.DirtyState,
    };
}
```

And the `CustomerRecordMutor`.

```csharp
public sealed class CustomerRecordMutor : RecordMutor<DmoCustomer>, IRecordMutor<DmoCustomer>
{
    [TrackState] public string? Name { get; set; }
    public override bool IsNew => BaseRecord.Id.IsNew;

    private CustomerRecordMutor(DmoCustomer record)
    {
        this.BaseRecord = record;
        this.SetFields();
    }

    private void SetFields()
    {
        this.Name = this.BaseRecord.Name.Value;
    }

    public override DmoCustomer Record => this.BaseRecord with
    {
        Name = new(this.Name ?? "No Name Set")
    };

    public void Reset()
        => this.SetFields();

    public static CustomerRecordMutor Load(DmoCustomer record)
        => new CustomerRecordMutor(record);

    public static CustomerRecordMutor NewMutor()
        => new CustomerRecordMutor(DmoCustomer.NewCustomer());
}
```
Notes:

1. It's a presentation domain object.
1. `[TrackState]` is a Blazor UI `EditStateTracker` Component attribute.
1. It can only be created through one of two static methods.
1. It tracks overall state.
1. It has an interface soit can be used in boilerplate generic forms.

Whle it's not *Pure*, it basically follows the same pattern as a *FP* `Map` function.  It takes in a `DmoCustomer` and outputs a `DmoCustomer`.
