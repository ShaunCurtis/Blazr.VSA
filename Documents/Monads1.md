# Monads

Type *Monad* into your search bar: the Internet is awash with articles.  There are even articles about the articles trying to explain why the original articles fail!


Here's another: but hopefully one that succeeds.

Why do they fail?  Perhaps because they try to explain this:

> A *Monad* is just a *Monoid* in the *Category* of *EndOfFunctors*.

Mathmatically correct (or so I'm informed), but meaningless to mere mortals.

Lets come at this from a different angle.

Here's a C# skeleton Monad:

```csharp
public record Monad<T>(T value) 
{
    Monad<TOut> ExecuteFunction(Func<T, Monad<TOut>> f);
}
```

It fulfills the basic requirments. i.e. it has:

1. A constructor - `new(value)` 
1. A generic method to execute a function with the `Func<T, Monad<TOut>>` pattern: known as a *Manadic Function*.

Now let'a look at a coding problem the Monad pattern will help us solve.

Consider this simple console application.

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

It works, but it's ugly.  

Why?  Well, you have to really look at this code to see what's going on.  `TryParse` spouts results at both ends: it returns a `bool` and outputs the parsed value via an `out` parameter.

Let's refactor it using the *Result Monad*.

## Result

First, our Monad.

```csharp
public record Result<T>
{
    public T? Value { get; private init; }
    public Exception? Exception { get; private init; }

    public Result(T value) : this(value, null) { }
    public Result(Exception exception) : this(default, exception) { }

    private Result(T? value, Exception? exception)
    {
        Value = value;
        Exception = exception;
    }

    public Result<TOut> ExecuteFunction<TOut>(Func<T, Result<TOut>> function)
        => this.Exception is null
            ? function(Value!)
            : new Result<TOut>(this.Exception);
}
```

A specific exception for it:

```csharp
public class ResultException : Exception
{
    public ResultException() : base("The Result is Failure.") { }
    public ResultException(string message) : base(message) { }

    public static ResultException Create(string message)
        => new ResultException(message);
}
```

`Result<T>` can be in one of two states:

- **Succeeded or HasValue**: The operation completed successfully and produced a Value.
- **Failed or HasException**: The operation failed, and the result contains an exception.

 Note: it's important the state check is on the `Exception` property.  `T` is not necessarily a `Nullable`: `int` will be set to `0`.

 The `ExecuteResult` method executes a function that takes a `T` and returns a new `Result<TOut>`.  If the input `Result<T>`:

 - Is in the *Succeeded* state, it executes the function and returns the result.  
 - Is in the *Failed* state, it short-circuits, returning a new `Result<TOut>` with the exception from the input `Result<T>`.  It doesn't execute the function.

## Refactoring

We can now start to refactor our console app using `Result<T>`.

> Note: To enter null in the console use `<Ctl>z <Enter>`.

We create a `Result<string?>` from the console read like this:

```csharp
new Result<string?>(Console.ReadLine())
```

And then execute a function against it:

```csharp
new Result<string?>(Console.ReadLine())
    .ExecuteFunction<double>((value) =>
    {
        if (value is null)
            return new Result<double>(ResultException.Create("The input value was null"));

        try
        {
            var output = double.Parse(value!);
            return (value is null)
                ? new Result<double>(ResultException.Create("The input value was nota number."))
                : new Result<double>(output);
        }
        catch (Exception ex)
        {
            return new Result<double>(ex);
        }
    }
);
```

### The Map Function

Look at the `MapFunction` code. `double.Parse` has the very common basic pattern `TIn -> TOut`.  We can boilerplate the pattern into our monad like this:

```csharp
public Result<TOut> MapFunction<TOut>(Func<T, TOut> function)
{
    if (this.Exception is not null)
        return new Result<TOut>(this.Exception!);

    try
    {
        var value = function.Invoke(this.Value!);
        return (value is null)
            ? new Result<TOut>(new ResultException("The function returned a null value."))
            : new Result<TOut>(value);
    }
    catch (Exception ex)
    {
        return new Result<TOut>(ex);
    }
}
```

It does two incredibly important things in program flow control:

  - If the result is in failure state, it constructs a new `Result<TOut>` passing in the existing exception.  It short circuits and doesn't execute `function`.
  - It sinks any raised exception and passes it on through the returned `Result<TOut>`. 


### The Create Function

The second very common pattern is `T? -> Result<T>` as in:

```csharp
return (value is null)
    ? new Result<double>(ResultException.Create("The input value was nota number."))
    : new Result<double>(output);
```

 we can boilerplate this:

```csharp
    public static Result<T> Create(T? value)
        => value is null
            ? new( default, ResultException.Create("Value was null"))
            : new(value) ;
```

### String Extensions

We can add a local extension to `string?` like this:

```csharp
public static class Extensions
{
    public static Result<string> ToResult(this string? value)
        => value is null
            ? new Result<string>(ResultException.Create("Value can'tbe null."))
            : new Result<string>(value);
}
```

### Output

Finally we need a mechanism to interact with I/O, such as writing to the console.  

`OutputValue` will return the value or return the value provided by the `Func`.

```csharp
    public T OutputValue(Func<Exception, T> hasException)
        => this.Exception is null
            ? this.Value!
            : hasException.Invoke(Exception!);

    public TOut OutputValue<TOut>(Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
        => this.Exception is null
            ? hasValue.Invoke(this.Value!)
            : hasException.Invoke(Exception!);
```

## Refactoring

We can now do some further refactoring to our code.

The code uses `OutputValue` to provide a value to the I/O.

```csharp
Console.WriteLine(
    Console.ReadLine()
   .ToResult()
   .MapFunction<double>(double.Parse)
   .OutputValue<string>(
        hasValue: (value) => $"Value is: {value}",
        hasException: (exception) => $"Error: {exception.Message}"
    ));
```

Adding more processes is simple.  Lets get the square root to two decimal places.

```csharp
Console.WriteLine(
    Console.ReadLine()
   .ToResult()
   .MapFunction<double>(double.Parse)
   .MapFunction(Math.Sqrt)
   .BindFunction(To2Decimals)
   .OutputValue<string>(
        hasValue: (value) => $"Value is: {value}",
        hasException: (exception) => $"Error: {exception.Message}"
    ));
```

### The Bind Function

The code above introduces `BindFunction` calling `To2Decimals`:

```csharp
Result<string> To2Decimals(double value)
    => Result<string>.Create(Math.Round(value, 2).ToString());
```

Note the pattern used `TIn -> Monad<TOut>`.  This is a very common pattern which we can boilerplate in `Result<T>`:

```csharp
public Result<TOut> BindFunction<TOut>(Func<T, Result<TOut>> function)
    => this.Exception is null
        ? function(Value!)
        : new Result<TOut>(this.Exception);
```

Note that you could have written this as a Llambda expression:

```csharp
.BindFunction(value => Result<string>.Create(Math.Round(value, 2).ToString()))
```


## So What [Hopefully] Have You Learnt from this Exercise

*Monads* are wrappers/containers: an implementation of the **Decorator Pattern** with some specific *functional* methods.  They provide high level generic functions abstracting underlying standard C# coded functionality. 

I find this the best definition I've seen:

> A monad is a design pattern that structures computations. It encapsulate values and operations, enabling: chaining; composition; and handling of side effects such as null values and errors.

### Functions

In FP the phrase "Functions are first class citizens" is used a lot.  C# has delegates and `Func` and `Action` implementations.  In OOP you will rarely see them used.

*Functions* are methods that take an input, apply one or more functions to that input, and produce an output.  *Functions* are the building blocks of FP.

In FP you apply functions to data and produce a result.  In OOP you pass data into methods to mutate state.

### Railway Orientated Programming

Whether you realised it or not, the FP patterns I've used implement *Railway Orientated Programming*.  If the input `Result<T>` is in *Exception* state, any function short-circuits, passing the exception to the output `Result<TOut>`: the function function is not executed.  Once on the *Exception* track, you stay there..

### High Level Features

There are many high level features built into C#: `Task` and `IEnumerable` are good examples.  The low level code C# code is very different from the code we type.  `Linq` is a library that adds Monadic functionality to `IEnumerable`.

`Result<T>` is no different.  It's high level code, *syntactic sugar*, that abstracts higher level functionality into lower level boilerplate code.

### Return, Bind, Map, Match

I've deliberately modified the standard FP terms for operations: I've used names that I believe are more descriptive.  I think sticking to the classic terms is counter-productive:  Return, Map, Bind, Match and many other have different means in OOP C#.

## Appendix

The `ResultException`:

```csharp
public class ResultException : Exception
{
    public ResultException() : base("The Result is Failure.") { }
    public ResultException(string message) : base(message) { }
}
```

