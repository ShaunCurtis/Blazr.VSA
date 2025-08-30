# Functional Programming in C#

If you're reading this you're interested in applying a more *Functional Programming* paradigm to your C# code.  Is so, you need to know about *Monads*.

**STOP**, don't turn off.  There's no confusing exlanations here, I'll show you how they work by developing one.

One of the most prevalent challenges in C# is handling and cascading nulls and exceptions.  How namy times do you write `if (value is null)...` and `catch...`.

Consider the `TryParse` function.  It's a horrible solution to the null and catch problem.  It spouts stuff at both ends.  What you need is a an output that can be either a value, or an error.  Here's the value, or here's a structured meesage that explains what went wrong.

This is the problem that a `Result` monad solves.

Consider this simple console app using `TryParse`:

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

It returns a `bool` and outputs the parsed value via an `out` parameter.  It's horrible.  You have to really look at this code to see what's going on.  The inputs and outputs are all mangled together.

Deconstruct the `TryParse` to get to this code:

```csharp
var value = Console.ReadLine();

if (value is null)
    Console.WriteLine("Value is Null.");
else
{
    try
    {
        var output = double.Parse(value!);
        Console.WriteLine($"Value is {output}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Value was not a number. Error: {ex.Message}.");
    }
}
```

Let's refactor it using a *Result Monad*.

## Basic Structure

First, our basic Monad.

```csharp
public record Result<T>
{
    private T? Value { get; init; }
    private Exception? Exception { get; init; }

    private Result(T? value, Exception? exception)
    {
        Value = value;
        Exception = exception;
    }
}
```


And an exception for result errors:

```csharp
public class ResultException : Exception
{
    public ResultException() : base("The Result is Failure.") { }
    public ResultException(string message) : base(message) { }

    public static ResultException Create(string message)
        => new ResultException(message);
}
```

Everything is private and immutable.

There are two public constructors:

```csharp
    public Result(T value) : this(value, null) { }
    public Result(Exception exception) : this(default, exception) { }
```

A static constructor to handle nulls:

```csharp
    public static Result<T> Create(T? value)
        => value is null
            ? new( default, ResultException.Create("Value was null"))
            : new(value) ;
```

And static constructor for errors:

```csharp
    public static Result<T> Error(string message)
        => new(default, ResultException.Create(message));
```

At this point we can get a result in the programme.

```csharp
var result = Result<string>.Create( Console.ReadLine());
```

Not very pretty, but adding a simple extension method to `string`:

```csharp
public static class ResultExtensions
{
    public static Result<string> ToResult(this string? value)
        => value is null
            ? new Result<string>(ResultException.Create("Value can'tbe null."))
            : new Result<string>(value);
}
```

We can:

```csharp
var result =  Console.ReadLine()
      ToResult();
```

## Function Handling

We add power to our monad by adding generic Function handling.

For our example we need to handle the `Func<T, TOut>` delegate pattern.  This is called a *Mapping Operation* in FP.

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

The method accepts a Function (C# method) that matches the `Func<T, TOut>`.  It first checks it's state.  If the last operation created a `Result<T>` with an exception, it wraps the exception in a new `Result<TOut>` and returns it.  It only executes `function` within a `try..catch` if it has a valid `T` value. if the operation completes successfully, it creates a new `Result<TOut>`with the valid result value.  If there's an exception raised, it sinks the exception and returns a new `Result<TOut>` with the exception. 

We can now call `MapFunction` and pass `Double.Parse` as the delegate function:

```csharp
var result = Console.ReadLine()
    .ToResult()
    .MapFunction(Double.Parse);
```

## Outputting

So far, so good, but we need to interact with the real world: to unwrap the value.

The `OutputValue` method does that.  There are several ways you may want to do this, so several variants of `Output`.

Here `OutputValue` produces a `TOut` provided by the two mapping functions provided.

```csharp
    public TOut OutputValue<TOut>(Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
        => this.Exception is null
            ? hasValue.Invoke(this.Value!)
            : hasException.Invoke(Exception!);
```

We'll use it to to provide a string, but I've used it for output various types including UI components, such as an Alert Control in Blazor.

We can now refactor the programme like this:

```csharp
Console.WriteLine(
    Console.ReadLine()
    .ToResult()
    .MapFunction(Double.Parse)
    .OutputValue<string>(
        hasValue: (value) => $"Success: The transformed value is: {value}",
        hasException: (ex) => $"Failure: {ex.Message}"
    ));
```

## Adding Processing

We want to add more `double` processing to our process.  Very simple, but I'll overcomplicate it slightly to show you what a  *Nomadic Function* is.

Let's add getting the square root and then reduction to two decimal places.

`Math.Sqrt` has the same Function signature as parse, but `Math.Round` doesn't.

Let's add a new method to `Result<T>`:

```csharp
    public Result<TOut> BindFunction<TOut>(Func<T, Result<TOut>> function)
        => this.Exception is null
            ? function(Value!)
            : new Result<TOut>(this.Exception);
```

This is the classic *Nomadic Function*: the pattern is `Func<T, Result<TOut>>`.  It takes a standard value, applies some processing and outputs a result.  Once you adopt the *FP*  paradigm, you'll write a lot of methods to this pattern.

Now we can refactor our programme:

```csharp
Console.WriteLine(
    Console.ReadLine()
    .ToResult()
    .MapFunction(Double.Parse)
    .MapFunction(Math.Sqrt)
    .BindFunction((value) => Result<double>.Create(Math.Round(value, 2)))
    .OutputValue<string>(
        hasValue: (value) => $"Success: The transformed value is: {value}",
        hasException: (ex) => $"Failure: {ex.Message}"
    ));
```
In this instance I would have used `MapFunction` but I wanted to demostrate *BindFunction* and the use of anonymous functions:

```csharp
    .MapFunction((value) => Math.Round(value, 2))
```


## Wrap Up

FP is about computing a result.  When you call an FP function you pass in a value and get back a result.  There should be no mutation of state, no side effects, and no changes to the input value.  The function takes the input, applies a function, and returns a new value.  It's known as a *Pure Function*.
