# Monads

Type *Monad* into your search bar.  The Internet is awash with articles.  There are even articles about the articles trying to explain why the original articles fail!

This is another: but hopefully for at least some of you it will succeed.

At this point let's forget we every heard the term *Monad*.  Erase it from memory.

Done: let's write some code.

A simple console app to calculate the square root of a number you enter.

```csharp
var input = Console.ReadLine();

if (double.TryParse(input, out double value))
{
    value = Math.Sqrt(value);
    Console.WriteLine($"The square root is: {Math.Round(value, 2)}");
}
else
{
    if(value == Double.NaN)
        Console.WriteLine($"The input is not a number");
    else if (Double.IsPositiveInfinity(value))
        Console.WriteLine($"The root is at Positive Infinity.");
    else
        Console.WriteLine($"The input is not a valid");
}
```

It works, but it's ugly.  
 - `TryParse` spouts results at both ends: it returns a `bool` and outputs the parsed value via an `out` parameter.  
 - There's stacked conditional statements.  
 - You have to really look at this code to see what's going on.

Can we do better?

Let's create an object to handle the two possible outcomes:

 - **Success** - we get a square root to two decimal places
 - **Failure** - we fail somewhere along the way and need to report why.  
 
 There are several ways to encapsulate the error message.  I use the `Exception` object: the reason will become evident shortly.

 Our new `Return<T>` object.

 It has two read only properties and three static constructors to ensure it's integrity.

 ```csharp
 public record Result<T>
{
    public T? Value { get; private init; }
    public Exception? Exception { get; private init; }

    public static Result<T> Success(T value) => new(value, null);
 
    public static Result<T> Failure(Exception exception) => new(default, exception);
 
    public static Result<T> Create(T? value)
        => value is null
        ? new Result<T>(default, new ArgumentNullException("T was null"))
        : new Result<T>(T, null);

    private Result(T? value, Exception? exception)
    {
        Value = value;
        Exception = exception;
    }
}
```

At this point we have a return object, but it's cumbersome to use and doesn't solve our problems.

So lets add a generic handler for a function like `Double.Parse` with a signature:

```csharp
Func<Tin, TOut>
```

Here's the function:  

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

It does two incredibly powerful things:

  - If the result is in fsilure state, it constructs a new `Result<TOut>` passing in the existing exception.  It short circuits and doesn't execute `function`.
  - It sinks any raised exception and passes it on through the returned `Result<TOut>`. 

We can now rewite our app code like this:

```csharp
var input = Console.ReadLine();


var result = Result<string>.Create(input)
    .ExecuteFunction(Double.Parse)
    .ExecuteFunction(Math.Sqrt);

if (result.Exception is null)
    Console.WriteLine($"The square root is: {Math.Round(result.Value, 2)}");
else
    Console.WriteLine($"Error: {result.Exception.Message}");
```

Note that we have now chained together `Double.Parse` and `Math.Sqrt`.  These convert a `string` input into a `double` output seamlessly. 

Next we can add a simple extension method to `string`:

```csharp
public static class StringExtensions
{
    public static Result<string> ToResult( this string? value)
        => Result<string>.Create(value);
}
```

And the app then looks like this:

```csharp
var result = Console.ReadLine()
    .ToResult()
    .ExecuteFunction(Double.Parse)
    .ExecuteFunction(Math.Sqrt);
```

At thia point our only remaininbg issue is the output.

We can address this by providing a way to output from `Result<T>`.

```csharp
public TOut OutputValue<TOut>(Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
    => this.Exception is null
        ? hasValue.Invoke(this.Value!)
        : hasException.Invoke(Exception!);
```

And out console app:

```csharp
Console.WriteLine(Console.ReadLine()
    .ToResult()
    .ExecuteFunction(Double.Parse)
    .ExecuteFunction(Math.Sqrt)
    .OutputValue<string>(
        hasValue: (value) => $"Value is: {value}",
        hasException: (exception) => $"Error: {exception.Message}"
    ));
```

One little problem, we've lost the rounding.  We can add this into the chain using `ExecuteFunction` and a simple lambda expression:

```csharp
Console.WriteLine(Console.ReadLine()
    .ToResult()
    .ExecuteFunction(Double.Parse)
    .ExecuteFunction(Math.Sqrt)
    .ExecuteFunction(value => Double.Round(value, 2))
    .OutputValue<string>(
        hasValue: (value) => $"Value is: {value}",
        hasException: (exception) => $"Error: {exception.Message}"
    ));
```

And that is almost a **Monad**.

We need to add a function that unlocks the real power of monads: `Func<T, Result<TOut>` pattern.  A function that takes a value of `T` and outputs a `Monad<TOut>`.  Seems complicated, but it is in fact trivial:

```csharp
public Result<TOut> ExecuteFunction<TOut>(Func<T, Result<TOut>> function)
    => this.Exception is null
        ? function(Value!)
        : Result<TOut>.Failure(this.Exception);
```

With this we can build more complex functions that can themselves return monads.

You could write:

```csharp
public static Result<double> GetASqrt(string? value)
    => value.ToResult()
        .ExecuteFunction(Double.Parse)
        .ExecuteFunction(Math.Sqrt)
        .ExecuteFunction(value => Double.Round(value, 2));
```

And the console app:

```csharp
Console.WriteLine(Console.ReadLine()
    .ToResult()
    .ExecuteFunction(StringExtensions.GetASqrt)
    .OutputValue<string>(
        hasValue: (value) => $"Value is: {value}",
        hasException: (exception) => $"Error: {exception.Message}"
    ));
```








