# Monads

Monads are a rich topic for articles.  The internet is awash with articles attempting to enlighten readers about Monads.

This is another: but instead of trying to explain what one is, we'll build one.

As C# OOP programmers we have less than satisfactory solutions for some thorny coding challenges.  We're stuck in the OOP paradigm.

Functional programming [**FP**] has better answers for many of these challenges.

Consider this ugly, horrible piece of platform code:

```csharp
public static bool TryParse(string? s, IFormatProvider? provider, out int result);
```

It spouts results at both ends!

And a classicaly coded console app using it:

```csharp
var input = Console.ReadLine();

bool isInt = int.TryParse(input, out int value);

// apply some transforms to the result of the parsing
double result = 0;
if (isInt)
{
    result = Math.Sqrt(value);
    result = Math.Round(result, 2);
}
//... later
if (isInt)
{
    Console.WriteLine($"Parsed successfully: The transformed value of {value} is: {result}");
}
else
{
    Console.WriteLine($"Failed to parse input: {input}");
}
```

A functionally programmed version would look something likr this:

```csharp
var input = Console.ReadLine();

input
    .Bind(int.Parse)
    .Bind(value => Math.Sqrt(value))
    .Bind(value => Math.Round(value, 2))
    .Match(
        success: value => Console.WriteLine($"Parsed successfully: The transform result of {input} is: {value}"),
        failure: ex => Console.WriteLine($"Failed to parse input: {ex.Message}"
    )); 
```

## The Result Monad

The `Result<T>` Monad will address two problems:

1. Handle nullable method returns.
2. Surface exceptions from the infrastructure and domain layers into the presentation layer.
   
It works well in the data pipeline context. 

A `Result<T>` has two possible states:

- **HasValue**: The operation completed successfully and produced a Value.
- **HasException**: The operation failed, and the result contains an exception. 

First, we define a generic container for normal types.  In **FP** almost everything is immutable, so it's a `record`.

```csharp
public record Result<T> { }
```

And a `Value` of `T` and an `Exception`.  I've made them public so `Result<T>` can be used within the `OOP` paradigm.

```csharp
public record Result<T>
{
    public readonly T? Value;
    public readonly Exception? Exception;
}
```

And two state properties:

```csharp
public bool HasException => Exception is not null;
public bool HasValue => Exception is null;
```

There are three private constructors.  We only want to allow object creation through static constructors.

```csharp
private Result(T? value)
    => this.Value = value;

private Result(Exception? exception)
    => this.Exception = exception ?? new ResultException("An error occurred. No specific exception provided.");

private Result()
    => this.Exception = new ResultException("An error occurred. No specific exception was provided.");
 }
```

## Creating/Initialising `Result<T>`

The basic pattern is:

```csharp
T > Monad<T>
```

#### Basic Constructors

There are three basic static constructors:

```csharp
public static Result<T> Success(T value) => new(value);
public static Result<T> Failure(Exception exception) => new(exception);
public static Result<T> Failure(string message) => new(new ResultException(message));
```

#### Logical Constructors

And one logical constructor to handle nulls:

```csharp
public static Result<T> Create(T? value) =>
    value is null
        ? new(new ResultException("T was null."))
        : new(value);
```

##### Monadic Functions

We can output a `Result<T>` directly from a method vsuch as this:

```csharp
Result<int> ParseForInt(string? value)
    => int.TryParse(value, out int result)
        ? Result<int>.Create(result)
        : Result<int>.Failure(new FormatException("Input is not a valid integer."));
```

We can then define as:

```csharp
var input = Console.ReadLine();
var result = Result<string>.Create(input);
```
or:

```csharp
var input = Console.ReadLine();
var result = ParseForInt(input);
```

## Working with `Result<T>`

Our console app now looks like this:

```csharp
var input = Console.ReadLine();
 result = ParseForInt(input);
```

How do we output this to the console?

### Outputting a Result

A standard implementation looks like this:

```csharp
public void Output(Action<T>? hasValue = null, Action<Exception>? hasException = null)
{
    if (this.Exception is null)
        hasValue?.Invoke(_value!);
    else
        hasException?.Invoke(_exception!);
}
```

We can use this to output to the console:

```csharp
var input = Console.ReadLine();

ParseForInt(value)
    // Output the result
    .Output(
        hasValue: (value) => Console.WriteLine($"Success: {value}"),
        hasException: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );
```

Note the lack of repetative null checking and the chaining of operations.

Try entering `<Ctl>z` in the console application: a null.  It's handled gracefully.

## Mapping

This is where the real power comes in:
1. Chaining operations. 
2. Handling nulls and exceptions in the chain using *Railway Orientated Programming*.

The basic pattern for a map is:

```csharp
    public Result<TOut> ApplyTransform<TOut>(Func<T, Result<TOut>> mapping)
    {
        if (_exception is null)
            return mapping(_value!);

        return Result<TOut>.Failure(_exception!);
    }
```

A deceptively simple piece of very powerful code.

Lets look at it in operation.  We've provided `ApplyTransform` with a lambda expression to calculate the square root of the input.

```csharp
var input = Console.ReadLine();

ParseForInt(value)
    // Applying a Mapping function
    .ApplyTransform((v) => Result<double>.Create(Math.Sqrt(v)))
    // Output the result
    .OutputResult(
        success: (value) => Console.WriteLine($"Success: {value}"),
        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );
```

Run the application entering numbers, letters and null `<CTL>z`.  The program works gracefully.  If the result of `ParseForInt` is in failure state, `ApplyTransform` creates a new `Result<double>` in failure state with the input result's exception.  The `mapping` function is not executed.

If the lambda expression is used a lot, it can be defined seperately:

```csharp
Result<double> SquareRoot(int value)
    => Result<double>.Create(Math.Sqrt(value));

// or

public static class Utilities
{
    public static Func<int, Result<double>> ToSquareRoot => (value)
        => Result<double>.Create(Math.Sqrt(value));
}
```

And then:

```csharp
var input = Console.ReadLine();

ParseForInt(value)
    // Applying a Mapping function
    .ApplyTransform(SquareRoot)
    // Output the result
    .OutputResult(
        success: (value) => Console.WriteLine($"Success: {value}"),
        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );
```

## So What Have We Learnt

Hopefully, you've realised that *Monads* are just wrappers/containers: nothing mythical.  They can be applied to any type.  They add functional coding patterns to the wrapped type.  They let you code in a different way.  They provide high level coding functionality. 

### Functions

Functions are methods that take an input, apply one or more transforms, and produce an output.
Functions are the building blocks of FP.  `Map`, `Bind` and more complex functional patterns pass around functions as arguments.

You will often hear the phrase "Functions are first class citizens".  In OOP you rarely pass methods as arguments into other methods.  In FP you do it all the time

In functional programming you apply functions to data.  In OOP programming you pass data into methods and objects.

### Railway Orientated Programming

Whether you realised it or not, FP patterns such as `Bind` and `Map` in `Result<T>` implement *Railway Orientated Programming*.  If the input `Result<T>` is in failure state, they take the exception and pass it on in the output `Result<TOut>`.  Once you've jumped onto the failure track you stay there.  Execution is safe because success code never gets executed once you're on the failure track.

### High Level Features

There are many high level features built into C#: `Task` and `IEnumerable` are good examples.  The low level code C# code is very different from the code we type.  `Linq` is a library that adds a lot of Monadic functionality to `IEnumerable`.

`Result<T>` is no different.  It's high level code, *syntactic sugar*, that abstracts higher level functionality into lower boilerplate code.

## Appendix

The `ResultException`:

```csharp
public class ResultException : Exception
{
    public ResultException() : base("The Result is Failure.") { }
    public ResultException(string message) : base(message) { }
}
```

