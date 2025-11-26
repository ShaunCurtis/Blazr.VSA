# C# Monads in Practice - Part 2 - Our Second Monad

In Part 1 we learned about the *Monad* pattern how to build a simple I/O monad.  In this article we'll develop a more complex nomad to handle:

1. Input Errors/Exceptions.
2. Nullable returns : `T? GetData(TId id)`

and provide a path for errors/exceptions to flow from input to output.

## Our Demo Project

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

I'm not using `TryParse` because I want to demonstrate catching exceptions. 

To fulfill our requirements, we need a tri-state monad:

1. True/Success
2. False/Failure
3. False/Failure with Exception

## `Bool<T>`

`Bool<T>` is an immutable record so we can use private `new` constructors with `private init` properties.  We have `static` contructors to tighlty control object initialization.

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

We provide a custom exception to pass messages.  The end point can differential bewteen a real exception and a message.

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

Three `Input` monadic constructors.

```csharp
public static Bool<T> Input(T? value)
    => value is null
    ? new(new BoolException("T was null."))
    : new(value);

public static Bool<T> Input(T? value, string errorMessage) =>
    value is null
        ? new(new BoolException(errorMessage))
        : new(value);

public static Bool<T> Input(Func<T?> input)
    => Input(input.Invoke());
```

And four statics on `BoolT`:

```csharp
public static class BoolT
{
    public static Bool<T> Success<T>(T value) => Bool<T>.Success(value);

    public static Bool<T> Input<T>(T? value)
    => Bool<T>.Input(value);

    public static Bool<T> Input<T>(T? value, string errorMessage)
        => Bool<T>.Input(value);

    public static Bool<T> Input<T>(Func<T?> input)
        => Input(input.Invoke());
}
```

### Map

The standard `Map` *Functor* function encapsulates a `try` to capture any exceptions in the execution of `function`.   

```csharp
extension<T>(Bool<T> @this)
{
    public Bool<TOut> Map<TOut>(Func<T, TOut> function)
    {
        if (@this.Exception is not null)
            return Bool<TOut>.Failure(@this.Exception!);

        try
        {
            var value = function.Invoke(@this.Value!);
            return (value is null)
                ? Bool<TOut>.Failure(new BoolException("The function returned a null value."))
                : BoolT.Input(value);
        }
        catch (Exception ex)
        {
            return Bool<TOut>.Failure(ex);
        }
    }
}
```

### Bind

The *Monadic function* implementation as an extension:

```csharp
extension<T>(Bool<T> @this)
{
    public Bool<TOut> Bind<TOut>(Func<T, Bool<TOut>> function)
        => @this.HasValue
            ? function(@this.Value!)
            : Bool<TOut>.Failure(@this.Exception!);
}
```

### Output

Finally an output method.  This one returns an `IOMonad`. 

```csharp
extension<T>(Bool<T> @this)
{
    public IOMonad<TOut> ToIOMonad<TOut>(Func<TOut> hasNoValue, Func<T, TOut>? hasValue = null, Func<Exception, TOut>? hasException = null)
        => (Tuple.Create(@this.HasValue, @this.HasException)) switch
        {
            (true, false) => IOMonad.Input(hasValue is not null ? hasValue.Invoke(@this.Value!) : default!),
            (false, true) => IOMonad.Input(hasException is not null ? hasException.Invoke(@this.Exception!) : default!),
            _ => IOMonad.Input(hasNoValue.Invoke()),
        };
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

