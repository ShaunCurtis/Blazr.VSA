# Monads

I'm not sure how to start *Not Yet Another Monad Article*, so let's forget the **M** word until the end of this article.  Instead, let's write some code to address two fundimental C# coding problems:

1. Passing errors through the system from *Input* [a database call, the user typing,...] to *Output* [data on a screen, an API response,...].
2. Handling nullable return values `List<T>? GetData(DataId id)` in a structured way.  No more `if (result is null) ...`

The aim is to remove the need to write hundreds/thousands of lines of defensive code:   No more `if (result is null) ...`..

## The simple Demo App

Consider this simple console app:

```csharp
Console.WriteLine(
    double.Parse(Console.ReadLine())
);
```

Enter `Twelve` and **BANG**:

```cshrp
twelve
Unhandled exception. System.FormatException: The input string 'twelve' was not in a correct format.
   at System.Number.ThrowFormatException[TChar](ReadOnlySpan`1 value)
   at System.Double.Parse(String s)
   at Program.<Main>$(String[] args) in C:\Users\Surface\source\Blazr\Blazr.VSA\Weather\Tests\Blazr.Tests.Console\Program.cs:line 6
```

So you change to `TryParse`:

```csharp
Console.WriteLine(
    double.TryParse(Console.ReadLine(), out double value)
);
```

which outputs:

```csharp
12
True
```

Where's `12`, the result you would expect?  

You need to examine the methods rear end.  `Tryxxxxx` belches and farts at the same time!   It's designed specifically for `if` statements.  

So why doesn't the language go further? Do the `if` wrapping for you?  It would if it could, but C# out-of-the-box doesn't support *discriminated unions*.  It's not baked into the language.  Yet, but, don't hold your breath:  we've been waiting a long time on that promise.

Instead we have the horrible `Tryxxxxx` cludge sprinkled through the C# libraries.

Let's do better.

We need a generic version of this:

```csharp
try
{
    Console.WriteLine(double.Parse(Console.ReadLine()!));
}
catch {
    Console.WriteLine("It looks like things went BANG!");
}
```

Wrapping a try within a `Func<In, Out>`.

A `Tuple<bool, T>' works, but applying the *Decorator Pattern* is more elegant.

Introducing `Bool<T>`.

## `Bool<T>`

`Bool<T>` combines a `bool` with a value of T when `true`.

This is the basic implementation:

```csharp
public readonly record struct Bool<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; private init; } = false;
    public T? Value { get; private init; } = default!;
    public Exception? Exception { get; private init; }

    public Bool() { }

    public Bool(T? value)
    {
        if (value is not null)
        {
            HasValue = true;
            Value = value;
        }
    }

    public Bool(Exception? exception)
        => Exception = exception;

    public static Bool<T> True(T value)
        => new Bool<T>(value);

    public static Bool<T> False()
        => new Bool<T>();

    public static Bool<T> False(Exception? exception)
        => new Bool<T>(exception);
}
```

We can now code a new `TryParse` returning a `Bool<double>`:

```csharp
Bool<double> TryParseString(string? value)
{
    if (value is null)
        return Bool<double>.False();

    try
    {
        return Bool<double>.True(double.Parse(value));
    }
    catch(Exception ex)
    {
        return Bool<double>.False(ex);
    }
}
```

The refactored console app:

```csharp
Console.WriteLine(
    TryParseString(Console.ReadLine())
);
```

Which produces:

```text
> 12
Bool { HasValue = True, Value = 12 }
```

We'll come back to this method shortly, but for now we'll build this functionality into `Bool<T>`. 

## Map

Breaking this down into two steps [we'll see why later]:  

**First** a simple `Map` function.  It takes a function with the pattern `Func<T, TOut>`:  for example `double.Parse`. 

```csharp
public Bool<TOut> Map<TOut>(Func<T, TOut> map)
    => this.HasValue
        ? Bool<TOut>.True(map.Invoke(this.Value))
        : Bool<TOut>.False(this.Exception);
```

If `Bool<T>`'s state is:

  -  *true*, it executes the `map` function and returns the result wrapped in a new `Bool<T>` instance.
  -  *false*, it returns a new *false* `Bool<Out>` instance and transfers over any exception.

**Second** the *Try* version, which simply wraps the `Map` in a try:

```csharp
public Bool<TOut> TryMap<TOut>(Func<T, TOut> map)
{
    try
    {
        return Map<TOut>(map);
    }
    catch
    {
        return Bool<TOut>.False(this.Exception);
    }
}
```

## Extending Existing Types

We need an elegant construct to get the console input into a `Bool<string>`: wrapping it.

There are several ways: my favourite in this case [until we get static extension methods] is another *Decorator Pattern* implementation: a helper wrapper around `Console.ReadLine`:

```csharp
public static class ConsoleHelper
{
    public static Bool<string> ReadLine()
    {
        string? input = Console.ReadLine();
        return new Bool<string>(input);
    }
}
```

The console app now refactors to:

```csharp
Console.WriteLine(
    ConsoleHelper.ReadLine()
    .TryMap(double.Parse)
);
```

And a test output:

```text
16
Bool { HasValue = True, Value = 16 }
```

## Functors

A definition [from Wikipedia]:

> In functional programming, a functor is a design pattern ... that allows a function to be applied to values inside a generic type without changing the structure of the generic type.

The *function* is normally called `Map`.

Here's our `Map`: we coded it in the first of the two steps above.

```csharp
public Bool<TOut> Map<TOut>(Func<T, TOut> map)
    => this.HasValue
        ? Bool<TOut>.True(map.Invoke(this.Value))
        : Bool<TOut>.False(this.Exception);
```

It takes a function that it applies to the internal `T` and produces a new `Bool<TOut>`.  It doesn't change the existing object's structure.  `TOut` can be a different type to `T` or the same.  

Bool(T) is a *Functor*.

### What do *Functors* give us?  

Consider this modified console app:

```csharp
Console.WriteLine(
    ConsoleHelper.ReadLine()
    .TryMap(double.Parse)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
);
```

which outputs:

```text
16
Bool { HasValue = True, Value = 4 }
```

or:

```text
sixteen
Bool { HasValue = False, Value = 0 }
```

*Functors* provide two very powerful abstractions:

1. **Chaining** of functions.  The ouput of one method is passed as the argument to the next method.
2. **Railway Orientated Programming** where subsequent steps are only executed if `Bool<T>` has a valid value.  Once *Tripped*, all subsequent steps short circuit.

We can see both in the above console app.   

## Outputting

Up to this point, we've just used the default `ToString()` method to output the `Bool<T>`.

Let's add some specific methods for outputting `T`.

The first takes a default value.  It outputs the internal value on `true` and the provided `defaultValue` on `false`.

```csharp
public T OutputValue(T defaultValue)
    => this.HasValue ? this.Value : defaultValue;
```

The second returns the result of `defaultFunction` on `false`.

```csharp
public T OutputValue(Func<T> defaultFunction)
    => this.HasValue ? this.Value : defaultFunction.Invoke();
```

The refactored console app:

```csharp
Console.WriteLine(
    ConsoleHelper.ReadLine()
    .TryMap(double.Parse)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Map<string>(value => $"Success: The transformed value is: {value}")
    .OutputValue(defaultValue: "The input value could not be parsed.")
);
```

## Monadic Functions

The basic pattern for a *Monadic Function* is `A -> Monad<B>` or `Func<T, Monad<TOut>>`.

Look at `TryParseString` from earlier:  it fits the pattern.

```csharp
Bool<double> TryParseString(string? value)
{
    if (value is null)
        return Bool<double>.False();

    try
    {
        return Bool<double>.True(double.Parse(value));
    }
    catch(Exception ex)
    {
        return Bool<double>.False(ex);
    }
}
```

It's similar to `Map`, but far more powerful: you're in full control of the state of the returned `Bool<T>`.

*Monadic Functions* are handled within Monads using a `Bind` function.  We can add this to `Bool<T>`:

```csharp
public Bool<TOut> Bind<TOut>(Func<T, Bool<TOut>> bind)
    => this.HasValue ? bind.Invoke(this.Value) : new Bool<TOut>();
```

The console app refactored to use it:

```csharp
Console.WriteLine(
    ConsoleHelper.ReadLine()
    .Bind(TryParseString)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Map<string>(value => $"Success: The transformed value is: {value}")
    .OutputValue(defaultValue: "The input value could not be parsed.")
);
```

```text
12
Success: The transformed value is: 3.46
```
```test
sixteen
The input value could not be parsed.
```

## Match

Match is similar to `OutputValue`, it unwraps the wrapper, but provides a lot more flexibility:

 - `TOut` can be a different type. In our example we switch from a `double` to a `string`.
 - the optional `hasException` provides access to the exception if it exists.

```csharp
public TOut Match<TOut>(Func<T, TOut> hasValue, Func<TOut> hasNoValue, Func<Exception, TOut>? hasException = null)
{
    if (this.Exception is not null && hasException is not null)
        return hasException.Invoke(this.Exception);

    if (this.HasValue && hasValue is not null)
        return hasValue.Invoke(this.Value);

    return hasNoValue!.Invoke();
}
```

The refactored console app:

```csharp
Console.WriteLine(
    ConsoleHelper.ReadLine()
    .Bind(TryParseString)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Match<string>(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.",
        hasException: ex => $"An error occurred: {ex.Message}"
    )
```

which produces this output:

```text
twelve
An error occurred: The input string 'twelve' was not in a correct format.
```

## Monads

This the best definition I've seen:

> A monad is a design pattern that structures computations. It encapsulate values and operations, enabling: chaining; composition; and handling of side effects such as null values and errors.

The basic requirements for a *Monad* are:

1. A constructor - `new(value)` 
1. A generic method to execute a function with the `Func<T, Monad<TOut>>` pattern: a *Manadic Function*.

Our `Bool<T>` has both.  It's a Monad.

## So What [Hopefully] Have You Learnt from this Article

*Monads* are just wrappers/containers that implement some specific *functional* methods.  They provide high level generic functions abstracting underlying standard C# coded functionality. 

### Functions

In FP "Functions are first class citizens".  C# has delegates and `Func` and `Action` implementations.  In OOP you rarely see them used, in FP they are everywhere.

*Functions* are methods that take an input, apply one or more transforms to that input, and produce an output.  *Functions* are the building blocks of FP.

In FP you apply functions to data and produce a result.  In OOP you pass data into methods which mostly mutate state.

### State

`Bool<T>` has three states:
1. True
2. False
3. False with an Exception

### Railway Orientated Programming

`Bool<T>` implements *Railway Orientated Programming*.  If the input `Bool<T>` is in *False* state, any function short-circuits, passing any exception from the owner to the output `Bool<TOut>`: the function is not executed.  Once on the *False* track, you stay there..

### High Level Features

There are many high level features built into C#: `Task` and `IEnumerable` are good examples.  The low level code C# code is very different from the code we type.  `Linq` is a library that adds Monadic functionality to `IEnumerable`.

`Bool<T>` is no different.  It's high level code, *syntactic sugar*, that abstracts higher level functionality into lower level boilerplate code.

## Appendix

The `BoolTException`:

```csharp
public class BoolTException : Exception
{
    public ResultException(string message) : base(message) { }
}
```
