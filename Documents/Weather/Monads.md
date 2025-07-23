# Monads

The internet is awash with articles trying to explain what a Monad is.  This is yet another: I hope I succeed where the rest fail.

As a C# OOP programmer you need to open your mind.  Forget the OOP dogma that has ruled your programming life.

Functional programming [**FP** from now on] requires a different way of thinking. It has solutions for coding problems that constantly vex OOP programmers.

Consider this ugly, horrible code [yes it's platform .Net code]:

```csharp
public static bool TryParse(string? s, IFormatProvider? provider, out int result);
```

It spouts results at both ends!

Here's a classicaly coded console app using it:

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

And a functionally programmed version.

```csharp
var input = Console.ReadLine();

input
    .Map(int.Parse)
    .Map(value => Math.Sqrt(value))
    .Map(value => Math.Round(value, 2))
    .Match(
        success: value => Console.WriteLine($"Parsed successfully: The transform result of {input} is: {value}"),
        failure: ex => Console.WriteLine($"Failed to parse input: {ex.Message}"
    ));
```

## The Result Monad

The `Result<T>` monad is a variation of the common `Maybe<T>` and `Option<T>` monads.  It's built to handle null and exceptions in data pipelines. 

A result has two possible states:
- **Success**: The operation completed successfully
- **Failure**: The operation failed, and the result contains an error message. 

Defined as either a Value of `T` or an `Exception`:

```csharp
public record Result<T>
{
    private readonly T? _value;
    private readonly Exception? _exception;
}
```

There are three private constructors: private to only allow object creation through static constructors.

```csharp
private Result(T? value)
    => _value = value;

private Result(Exception? exception)
    => _exception = exception ?? new ResultException("An error occurred. No specific exception provided.");

private Result()
    => _exception = new ResultException("An error occurred. No specific exception was provided.");
 }
```

## Creating/Initialising `Result<T>`

Three basic static initialization methods on `Result<T>`:

```csharp
public static Result<T> Success(T value) => new(value);
public static Result<T> Failure(Exception exception) => new(exception);
public static Result<T> Failure(string message) => new(new ResultException(message));
```

The basic template for creating a `Result<T>` can be expressed like this:

```csharp
    T > Monad<T>
```

Which we use in the creator that deals with nulls:

```csharp
public static Result<T> Create(T? value) =>
    value is null
        ? new(new ResultException("T was null."))
        : new(value);
```

And can alse be used directly like this:

```csharp
Result<int> ParseForInt(string? value)
    => int.TryParse(value, out int result)
        ? Result<int>.Create(result)
        : Result<int>.Failure(new FormatException("Input is not a valid integer."));
```

We can then define:

```csharp
var input = Console.ReadLine();

var result = Result<string>.Create(input);
//or
var result = ParseForInt(input);
```


## Working with `Result<T>`

Our console app now looks like this:

```csharp
var input = Console.ReadLine();

var result = ParseForInt(input);
```

### Outputting a Result

Next we need to handle the Monad in `Console.WriteLine`.

A standard implementation looks like this:

```csharp
public void OutputResult(Action<T>? success = null, Action<Exception>? failure = null)
{
    if (_exception is null)
        success?.Invoke(_value!);
    else
        failure?.Invoke(_exception!);
}
```

The console app now looks like this:

```csharp
var input = Console.ReadLine();

ParseForInt(value)
    // Output the result
    .OutputResult(
        success: (value) => Console.WriteLine($"Success: {value}"),
        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );
```

Note the lack of null checking and the chaining of operations.

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

