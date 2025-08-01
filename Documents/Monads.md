# Monads

Type *Monad* into your search bar.  The Internet is awash with articles.  There are even articles about the aeticles tryig to explain why the original articles fail!


Here's another: but hopefully one that succeeds.

Here's why they fail:

> A *Monad* is just a *Monoid* in the *Category* of *EndOfFunctors*.

The statement is mathmatically correct (or so I'm informed), but if you understand it you're in a serious minority.

Articles fail to enlighten us because they try to explain that statement in plain language.

I know you can't.

So, lets come at this from a different angle.

Here's a C# base level Monad:

```
public record Monad<T>(T value) 
{
    Monad<TOut> ExecuteFunction(Func<T, Monad<TOut>> f);
}
```

It:
1. Has a constructor - `new(value)` 
1. A generic method to execute a function with the `Func<T, Monad<TOut>>` pattern: known as a *Manadic Function*.

Now let'a look at a coding problem that the Monad Pattern helps us solve.

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

It works, but it's ugly.  `TryParse` spouts results at both ends: it returns a `bool` and outputs the parsed value via an `out` parameter.  You have to really look at this code to see what's going on.

Let's refactor it using a *Result Monad*.

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

And a specific exception for it:

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

- **HasValue**: The operation completed successfully and produced a Value.
- **HasException**: The operation failed, and the result contains an exception.

 Note: it's important the state check is on the `Exception` property.  `T` is not necessarily a `Nullable`: `int` will be set to `0`.

 The `ExecuteResult` method executes a function that takes a `T` and returns a new `Result<TOut>`.  If the input `Result<T>`:

 - Is in the *HasValue* state, it executes the function and returns the result.  
 - Is in the *HasException* state, it short-circuits, returning a new `Result<TOut>` with the exception from the input `Result<T>`.  It doesn't execute the function.

We can now refactor our console app to use the `Result<T>` Monad:

> Note: To enter null in the console use `<Ctl>z <Enter>`.

```csharp
new Result<string?>(Console.ReadLine())
    .ExecuteFunction<double>((value) =>
    {
        if (value is null)
            return new Result<double>(ResultException.Create("The imput value was null"));

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

If we look at the code above there are three common patterns we use:

1. `T -> apply function TOut`
1. `exception -> apply function -> TOut`
1. `T? -> Result<T>`

We can code both of these into our Nomad:

```csharp
public Result<TOut> ExecuteFunction<TOut>(Func<T, TOut> function)
{
    if (this.Exception is not null)
        return Result<TOut>.Failure(this.Exception!);

    try
    {
        var value = function.Invoke(this.Value!);
        return (value is null)
            ? Result<TOut>.Failure(new ("The function returned a null value."))
            : Result<TOut>.Create(value);
    }
    catch (Exception ex)
    {
        return Result<TOut>.Failure(ex);
    }
}
```

```csharp
public T OutputValue(Func<Exception, T> hasException)
    {
        if (this.Exception is not null)
            return hasException.Invoke(Exception!);

        return this.Value!;
    }
```




## So What Have We Learnt from this Exercise

*Monads* are wrappers/containers: just an implementation of the **Decorator Pattern** with some specific *functional* methods.  They provide high level coding functionality, abstracting underlying standard C# coded functionality. 

### Functions

In FP you will hear the phrase "Functions are first class citizens" a lot.  C# has delegates and `Func` and `Action` implementations.  In OOP you will rarely see them used.

In FP, *Functions* are methods that take an input, apply one or more functions to that input, and produce an output.  *Functions* are the building blocks of FP.

In functional programming you apply functions to data and produce a result.  In OOP you pass data into methods to mutate the state of objects.

### Railway Orientated Programming

Whether you realised it or not, FP patterns implement *Railway Orientated Programming*.  If the input `Result<T>` is in *Exception* state, any function short-circuits, passing the exception to the output `Result<TOut>`: the function function is not executed.  Once on the *Exception* track, you stay there..

### High Level Features

There are many high level features built into C#: `Task` and `IEnumerable` are good examples.  The low level code C# code is very different from the code we type.  `Linq` is a library that adds Monadic functionality to `IEnumerable`.

`Result<T>` is no different.  It's high level code, *syntactic sugar*, that abstracts higher level functionality into lower level boilerplate code.

### Bind, Map, Match

I've deliberately not used the standard FP terms for operations: I've used names that I believe are more descriptive.  I think sticking to the classic terms is counter-productive:  Map, Bind, Match and many other have different means in OOP C#.

## Appendix

The `ResultException`:

```csharp
public class ResultException : Exception
{
    public ResultException() : base("The Result is Failure.") { }
    public ResultException(string message) : base(message) { }
}
```

