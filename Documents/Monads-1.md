# Yet another Monad Tutorial

This article started lif as "Functional Programming in C#".  The problem with this was the articles is really about Monads without actually mentioning the dreaded word.

Problem: How do you get referenced in searches about Monads, if you avoid using he word until the finale?

So this first secton is all about why Monads are so enigmatic [and for the consumption of you know what engines].

Read on, or probalby best, jump to the *Continors* section.

This quote interests me:

> The monadic curse is that once someone learns what monads are and how to use them, they lose the ability to explain them to other people.

Not sure why, but maybe they try and explain this:

> A Monad is just a Monoid in the Category of EndofFunctors.

My answer to that is "Life is too short".

If you're reding this then you've probably already tried some of the other articles.  This one isn't top of the search list.  Most quickly become incomprehensible.  There are a few gems of information out there, but they're not on page 1 of the search engine results, or are buried away in a mass of otherwise incomprehensible alpha numeric characters.

So before we begin,  That's it for the M word until close to the end of this article.

## The FP Progrsmming Paradigm

As I step through code examples in this article, I'll introduce various *FP* concepts.

You'll need to recognise and then learn and master them.  Many have crept into *C#* over the years, and you'll already be using them without knowing it.

## Containors

One of the most important *FP* concepts is *Higher Order Objects*: what I'll call *Containors* from now on.  These can be expressed as:

```csharp
Containor<T>
```

In *FP* they are immutable, so either:

```csharp
public record Containor<T> {...}
```

or 

```csharp
public readonly record struct Containor<T> {...}
```

Unless you're still in the coding dark ages, you're already using *containors*.  `IEnumerable<T>` and `Task<T>` are two examples.

*Containors* provide the mechanism for adding functionality to any existing object.  The *containor* type depends on the functionality added.

We can define the basic *containor* pattern:

```csharp
public readonly record struct Containor<T>
{
    public T Value { get; init; }

    public Containor(T value)
    {
            this.Value = value;
    }
}
```

The rest of this article will take the following simple console application and work some FP magic with *containors*.

```csharp
var input = Console.ReadLine();

if(double.TryParse(input, out double value))
{
    value = Math.Sqrt(value);
    Console.WriteLine($"The square root is: {Math.Round(value, 2)}");
}
else
{
    Console.WriteLine($"The input is not a valid");
}
```

First our *Containor*: called `Null<T>`. 

```csharp
public readonly record struct Null<T>
{
    public T? Value { get; init; } = default!;

    [MemberNotNullWhen(true, nameof(Value))]
    internal bool HasValue { get; init; } = false;

    public Null(T? value)
    {
        if (value is not null)
        {
            Value = value;
            HasValue |= true;
        }
    }
}
```
Note: `[MemberNotNullWhen(true, nameof(Value))]` is a compiler instruction on how to deal with nulls on `Value`.

By itself this doesn't do much.  An object with a `new` constructor, but no built in functionality.

## Containor Creation

You'll see this given many names: *Return*, *Lift*, *Raise*, *Elevate* are just a few.  I use *Read*.

We code two static constructor methods:

```csharp
    public static Null<T> Read(T Value)
        => new Null<T>(Value);

    public static Null<T> NoValue(T Value)
    => new Null<T>();
```

And a factory class:

```csharp
public static class NullT
{
    public static Null<T> Read<T>(Func<T> input)
        => new Null<T>(input.Invoke());

    public static Null<T> Read<T>(T value)
        => new Null<T>(value);

    public static Null<T> NoValue<T>()
        => new Null<T>();
}
```

So we can now. 

```csharp
NullT.Read(Console.ReadLine)
```

This demonstrates an important *FP* concept  The constructor used is:

```csharp
public static Null<T> Read<T>(Func<T> input)
```

The code passes a function `Console.ReadLine` as an argument to `Read`.  The delegate pattern for the function is `Func<T>`.   

The method code is: 

```csharp
new Null<T>(input.Invoke())
```

which constructs an instance of `Null<string>` from the output of `Console.ReadLine`.

Passing functions as arguments is part of the *FP* paradigm:

 - Functions are first class citizens. 
 - Functions are values.

Get use to `Func` and `Action`: they are used a lot.

## Outputting

There are two possible execution tracks to code for based on whether a value exists.

We can code a `Write` method to handle this:

```csharp
public TOut Write<TOut>(Func<T, TOut> hasValue, Func<TOut> hasNoValue )
    => this.HasValue
        ? hasValue.Invoke(Value)
        : hasNoValue.Invoke();
```

Again functions as arguments.  It's used like this:

```csharp
NullT.Read(Console.ReadLine)
    .Write(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.")
```

`HasValue` and `hasNoValue` are passed an lambda expressions.  I've "long form coded" this so you can see the proper argument assignment.

The retun value is a `string` which we can deal with using a simple functional extension to `string`.

```csharp
extension(string @this)
{
    public void Write(Action<string> writer)
        => writer.Invoke(@this);
}
```

Again a function as an argument.  This time we pass in `Console.WriteLine` as `writer`.

```csharp
NullT.Read(Console.ReadLine)
    .Write(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.")
    .Write(Console.WriteLine);
```

## Chaining

> The ability to *chain* or *sequence* functions - think LINQ `.Where(...).Take(...).Select...()` - to produce a *fluent* instruction flow.  

Let's now look at that awful `TryParse` that produces output at both ends.  It a hack because the necessary functionality isn't in the language.

We can write something like this to encapsulate it:

```csharp
static Null<double> TryParseToDouble(string? value)
{
    if (double.TryParse(value, out double result))
        return NullT.Read(result);

    return NullT.NoValue<double>();
}
```

Note the pattern `Func<T, Null<TOut>>`: pass in a `T`. get out a `Null<TOut>`.

We can bake this pattern into `Null<T>`:

```csharp
public Null<TOut> Bind<TOut>(Func<T, Null<TOut>> func)
    => HasValue 
        ? func.Invoke(Value) 
        : new Null<TOut> { HasValue = false };
```

Look closely at this implementation.  It only executes `func` if a value exists.  No value, it returns a new `Null<TOut>` with no value.

And then use it like this:

```csharp
NullT.Read(Console.ReadLine)
    .Bind(TryParseToDouble)
    .Write(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.")
    .Write(Console.WriteLine);
```

Note how we've just plugged it into the pipeline and it works.  The `Null<T>` type has changed from `string` to `double` but the functionality provided by the *containor* in generic.

**At this point `Null<T>` is a *Monad*.  You've written a *Monad* without knowing it.**

So what's a Monad?

The pattern `Func<T, Monad<TOut>>` is called a *Nomadic Function* and is the only really important fact you need to know about Nomads.  The method that implements it is normally called *Bind*.

Technically, there are only two requirements that a *Higher Level Object* needs to implement to be a *Monad*:

1. A method to raise a normal type into the *Containor*.  Often called `Return`, but a lot of programmers don't like that, so call it something else.  Our `Null<T>` uses `Read`.

2. A `Bind` method to process a *Monadic Function*. 

We can summarise this:

```csharp
class Monad<T> 
{
    Monad(T instance);
    Monad<TOut> Bind(Func<T, Monad<TOut>> f);
}
```

## The Functor Pattern

The other pattern we need is the *Functor* pattern: a way to process a bulk standard `Func<T,TOut>` such as `Math.Sqrt` or `Math.Round`.  It's normally called `Map`:

```csharp
class Functor<T> 
{
    Functor(T instance);
    Functor<TOut> Map(Func<T, TOut> f);
}
```

Our `Null<T>` implementation.  It follows the same pattern as `Bind` in only executing `func` if a value exists.  The difference is it needs to wrap the return in a `Null<TOut>`.

```csharp
public Null<TOut> Map<TOut>(Func<T, TOut> func)
    => HasValue 
        ? Null<TOut>.Read(func.Invoke(Value)) 
        : new Null<TOut>();
```

We can now:

```csharp
NullT.Read(Console.ReadLine)
    .Bind(TryParseToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.")
    .Write(Console.WriteLine);
```

Note how the two input argument `Math.Round` is handled in a llambda expression.

## Functions

A word about functions before we finish.

*Functions* are methods that take an input, apply one or more transforms to that input, and produce an output.  *Functions* are the building blocks of FP.

In FP you apply functions to data and produce a result.  In OOP you pass data into methods which mostly mutate state.

Functions should be pure: no side effects, no state changes.

## Wrap Up

There are several key point to take away with you:

1. *Monad* and *Functor* as just generic container patterns.  Not rocket science.

2. The patterns provide a convenient and concise way to encapsulate common coding chores (like typing out endless `if (x is null)....`).

3. The way `Bind` and `Map` work in `Null<T>` they implement *Railway Orientated Programming*.  Once a step in the pipeline fails (in our case returns a `null`), all the following steps are short circuited. 

## Full Code

```csharp
public readonly record struct Null<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    internal bool HasValue { get; init; } = false;
    internal T? Value { get; init; } = default!;

    public Null(T? value)
    {
        if (value is not null)
        {
            Value = value;
            HasValue |= true;
        }
    }

    public static Null<T> Read(T Value)
        => new Null<T>(Value);

    public static Null<T> NoValue(T Value)
    => new Null<T>();

    public T Write(T defaultValue)
        => HasValue ? Value : defaultValue;

    public void Write(Action<T> hasValue, Action? hasNoValue = null)
    {
        if (HasValue)
        {
            hasValue.Invoke(Value);
            return;
        }

        hasNoValue?.Invoke();
    }

    public TOut Write<TOut>(Func<T, TOut> hasValue, Func<TOut> hasNoValue )
        => this.HasValue
            ? hasValue.Invoke(Value)
            : hasNoValue.Invoke();

    public Null<TOut> Bind<TOut>(Func<T, Null<TOut>> func)
        => HasValue ? func.Invoke(Value) : new Null<TOut> { HasValue = false };

    public Null<TOut> Map<TOut>(Func<T, TOut> func)
        => HasValue ? Null<TOut>.Read(func.Invoke(Value)) : new Null<TOut>();
}

public static class NullT
{
    public static Null<T> Read<T>(Func<T> input)
        => new Null<T>(input.Invoke());

    public static Null<T> Read<T>(T value)
        => new Null<T>(value);

    public static Null<T> NoValue<T>()
        => new Null<T>();

    extension<T>(Null<T> @this)
    {
        public Bool<T> ToBoolT()
            => @this.HasValue
                ? Bool<T>.Success(@this.Value)
                : Bool<T>.Failure();
    }
}
```

```csharp
// Program.cs

NullT.Read(Console.ReadLine)
    .Bind(TryParseToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.")
    .Write(Console.WriteLine);

static Null<double> TryParseToDouble(string? value)
{
    if (double.TryParse(value, out double result))
        return NullT.Read(result);

    return NullT.NoValue<double>();
}

public static class Extensions
{
    extension(string? value)
    {
        public Null<double> TryParseToDouble()
        {
            if (double.TryParse(value, out double result))
                return NullT.Read(result);

            return NullT.NoValue<double>();
        }
    }

    extension(string @this)
    {
        public void Write(Action<string> writer)
            => writer.Invoke(@this);
    }
}
```

