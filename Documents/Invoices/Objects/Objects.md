# Objects

The types of objects you use has a huge impact on your project.  There's a whole world outside `public class MyClass` that will make your application faster, less buggy, and easier to code. 

Let's look at some code.

We could define our invoice like this:

```csharp
public class Invoice 
{
    [Key] public Guid InvoiceID { get; init; }
    public Guid CustomerID { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime Date { get; init; }
}
```

It reflects the database record, it can be edited and usedin the UI.  Why complicate matters, **KISS**.

There are several problems with this approach.

1. It's wide open to mutation, and therefore inadvertant changes that generate bugs.
2. Where do you validate the values?  it `decimal.min` a valid Total Amount? Is `Guid.Empty` a valid InvoiceID?
3.  Is inheritance valid?
4. Passing a `CustomerID` to a get record method is valid:  they are both `Guid` types.
5. *Primitive Obsession* rules.  The properties use primitives, such as Guids and decimal, to represent domain concepts. 

> Advice 1: Generic fit all data types are **BAD**.  Forget **KISS**.

> Advice 2:  Don't let storage dictate your data structure.

So, let's redefine our *Domain* object:

```csharp
public sealed record DmoInvoice
{
    public InvoiceId Id { get; init; } = InvoiceId.Default;
    public InvoiceCustomer Customer { get; init; } =  InvoiceCustomer.Default;
    public Money TotalAmount { get; init; } = Money.Default;
    public Date Date { get; init; }
}
```

1. The object is now a `record`: read only.
2. The object is `sealed`: there's no valid reason to inherit from it.  There are also small performance benefits in using `sealed` objects.
3. There are no primitives.  Instead there are value objects with validation built in.

Let's look at `Money` as an example value object:

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

It:
1. Is a read only `struct` i.e. a stack based object with value semantics. 
1. Validation built it: you can't have a negative amount.
1. A Custom `ToString` method.
1. Mathmatical operstors.



