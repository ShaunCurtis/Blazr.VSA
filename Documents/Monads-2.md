# C# Monads Part 2 - Our Second Monad

In Part 1 we learnt about *Containors*, *Monads* and *Functors* in programming terms:  They are just coding patterns.  We also learnt how to build a simple *Null* *Containor* that implemented the *Monad* and *Functor* patterns.

In this article I'll show you how to develop a more complex version of the `Null<T>` *containor* to handle both:

1. Nullable returns : as implemented ib `Null<T>` 
1. Input Errors/Exceptions.

and provide a path to flow those errors/exceptions from input to output: in a data pipeline, from the data store calls to the UI or API interface.

## Our Demo Project

This is basically the same as the first article.

```csharp
var input = Console.ReadLine();
try
{
    var result = double.Parse(input!);
    result = Math.Sqrt(result);
    result = Math.Round(result, 2);
    Console.WriteLine($"The result is {result}");
}
catch
{
    Console.WriteLine("The input value could not be parsed.");
}
```

I've used `parse`, not `TryParse`, to demonstrate catching exceptions. 

To fulfill our requirements, we need a tri-state monad:

1. True/Success
2. False/Failure
3. False/Failure with Exception

## `Bool<T>`

The basic *Containor* definition

```csharp
public sealed record Bool<T>
{
    public T? Value { get; private init; }
    [MemberNotNullWhen(true, nameof(Value))] public bool HasValue{ get; private init; }
    public Exception? Exception { get; private init; }

    public bool HasException => Exception is not null;
}
```

Notes:
1. Sealed - there's no valid reason to inherit.
2. It's a `record` rather than a `readonly record struct` because we need constructor control.
3. Compiler help attributes to qualify nullable state.

### Custom Exception

We need a custom exception to pass messages so the end point can differential between a real exception and a message.

```csharp
public class BoolException : Exception
{
    public BoolException() : base("The Bool operation failed.") { }
    public BoolException(string message) : base(message) { }

    public static BoolException Create(string message)
        => new (message);
}
```

### Constructors

The `new` constructors are private.

```csharp
private Bool(T value)
{
    Value = value;
    HasValue = true;
}

private Bool(Exception? exception)
    => Exception = exception;

private Bool()
    => this.HasValue = false;
```

There are four basic static constructors:

```csharp
    public static Bool<T> Success(T value) => new(value);
    public static Bool<T> Failure() => new();
    public static Bool<T> Failure(Exception exception) => new(exception);
    public static Bool<T> Failure(string message) => new(new BoolException(message));
```

And four `Read` constructors.

```csharp

    public static Bool<T> Read(T? value)
        => value is null
        ? new(new BoolException("T was null."))
        : new(value);

    public static Bool<T> Read(T? value, string errorMessage) =>
        value is null
            ? new(new BoolException(errorMessage))
            : new(value);

    public static Bool<T> Read(object? value = null  )
        => new();

    public static Bool<T> Read(Func<T?> input)
        => Read(input.Invoke());
```

And four statics on `BoolT`:

```csharp
public static class BoolT
{
    public static Bool<T> Success<T>(T value) => Bool<T>.Success(value);

    public static Bool<T> Read<T>(T? value)
    => Bool<T>.Read(value);

    public static Bool<T> Read<T>(T? value, string errorMessage)
        => Bool<T>.Read(value);

    public static Bool<T> Read<T>(Func<T?> input)
        => Read(input.Invoke());
}
```

### Map

The standard `Map` *Functor* function and a `TryMap` that encapsulates a `try` to capture any exceptions in the execution of `function`.   

```csharp
public Bool<TOut> Map<TOut>(Func<T, TOut> function)
{
    if (@this.Exception is not null)
        return Bool<TOut>.Failure(@this.Exception!);

    var value = function.Invoke(@this.Value!);
    return (value is null)
        ? Bool<TOut>.Failure(new BoolException("The function returned a null value."))
        : BoolT.Read(value);
}

public Bool<TOut> TryMap<TOut>(Func<T, TOut> function)
{
    if (@this.Exception is not null)
        return Bool<TOut>.Failure(@this.Exception!);

    try
    {
        var value = function.Invoke(@this.Value!);
        return (value is null)
            ? Bool<TOut>.Failure(new BoolException("The function returned a null value."))
            : BoolT.Read(value);
    }
    catch (Exception ex)
    {
        return Bool<TOut>.Failure(ex);
    }
}
```

### Bind

The *Monadic function* implementation as an extension:

```csharp
public Bool<TOut> Bind<TOut>(Func<T, Bool<TOut>> function)
    => @this.HasValue
        ? function(@this.Value!)
        : Bool<TOut>.Failure(@this.Exception!);
```

### Output

Finally a set of six `Write` methods. 

```csharp
public Bool<T> Write(Action<T>? hasValue = null, Action<Exception>? hasException = null)
{
    if (boolMonad.HasValue)
        hasValue?.Invoke(boolMonad.Value!);
    else
        hasException?.Invoke(boolMonad.Exception!);

    return boolMonad;
}

public T Write(Func<Exception, T> hasException)
    => boolMonad.HasException
        ? hasException.Invoke(boolMonad.Exception!)
        : boolMonad.Value!;

public T Write(Func<T> exceptionValue)
    => boolMonad.HasException
        ? exceptionValue.Invoke()
        : boolMonad.Value!;

public TOut Write<TOut>(Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
    => boolMonad.HasValue
        ? hasValue.Invoke(boolMonad.Value!)
        : hasException.Invoke(boolMonad.Exception!);

public T Write(T defaultValue)
    => boolMonad.HasValue
        ? boolMonad.Value!
        : defaultValue;

public Bool<T> Write(Action<Bool> Action)
{
    Action.Invoke(boolMonad.ToBool());
    return boolMonad;
}
```

 ## FP Demo App

First we need the most efficient parser:  we wrap the very ugly `TryParse` in a *Monadic Function* which we can then use with `Bind`.

 ```csharp
public Bool<double> TryParseString(string? value)
{
    if (double.TryParse(value, out double result))
        return BoolT.Success(result);

        return Bool<double>.Failure($"Failed to parse {value}");
}
```

And then refactor our app:  

```csharp
BoolT.Input(Console.ReadLine)
    .Bind(TryParseString)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .ToIOMonad(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.",
        hasException: ex => $"An error occurred: {ex.Message}")
    .Output(Console.WriteLine);
```

