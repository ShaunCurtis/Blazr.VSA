# Monads

Monads are a rich topic for conversation: the Internet is awash with Monad articles.  This is another: but I'll show you what one is, by build one.

As C# OOP programmers we have less than satisfactory solutions for some thorny coding challenges.  We're stuck in the OOP paradigm. Functional programming [**FP**] has better answers for many of these challenges.

Consider this ugly, horrible piece of platform code:

```csharp
public static bool TryParse(string? s, IFormatProvider? provider, out int result);
```

Is spouting results at both ends good practice?  Not in my book.

Here's a classically coded console app using it:

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
    Console.WriteLine($"Parsed successfully: The transformed value is: {result}");
}
else
{
    Console.WriteLine($"Failed to parse input: {input}");
}
```

And the FP version we'll develop:

```csharp
Console
    .ReadLine()
    .ParseForInt()
    .ApplyTransform((value) => Math.Sqrt(value))
    .ApplyTransform((value) => Math.Round(value, 2))
    .Output(
        hasValue: (value) => Console.WriteLine($"Parsed successfully: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failed to parse input: {ex.Message}")
    );
```

> Note: To enter null in the console use `<Ctl>z <Enter>`.


## The Result Monad

The `Result<T>` Monad will address two problems:

1. Handle nullable method returns.
2. Surface exceptions from the infrastructure and domain layers into the presentation layer.
   
It works well in the data pipeline context. 

A `Result<T>` has two possible states:

- **HasValue**: The operation completed successfully and produced a Value.
- **HasException**: The operation failed, and the result contains an exception. 

A Monad is a generic container.  In **FP** almost everything is immutable, so it's declared as  a `record`.

```csharp
public record Result<T> { }
```

`Value` of `T` and an `Exception` are properties: public so `Result<T>` can be used within the `OOP` paradigm.

```csharp
public record Result<T>
{
    public readonly T? Value;
    public readonly Exception? Exception;
}
```

State exposed as readonly properties:

```csharp
public bool HasException => Exception is not null;
public bool HasValue => Exception is null;
```

Three private constructors so we restrict object creation to static constructors.

```csharp
private Result(T? value)
    => this.Value = value;

private Result(Exception? exception)
    => this.Exception = exception ?? new ResultException("An error occurred. No specific exception provided.");

private Result()
    => this.Exception = new ResultException("An error occurred. No specific exception was provided.");
 }
```

## Creating/Initialising a `Result<T>`

The basic pattern is:

```csharp
T > Monad<T>
```

Basic static constructors:

```csharp
public static Result<T> Success(T value) => new(value);
public static Result<T> Failure(Exception exception) => new(exception);
public static Result<T> Failure(string message) => new(new ResultException(message));
```

A logical constructor to handle nulls:

```csharp
public static Result<T> Create(T? value) =>
    value is null
        ? new(new ResultException("T was null."))
        : new(value);
```

We can now re-write our console app:

```csharp
using Blazr.Manganese;

var input = Console.ReadLine();

Result<int> parseResult;

if (int.TryParse(input, out int value))
{
    parseResult = Result<int>.Create(value);
}
else
{
    parseResult = Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
}

if (parseResult.HasValue)
{
    double transformedValue = Math.Sqrt(parseResult.Value);
    transformedValue = Math.Round(transformedValue, 2);
    Console.WriteLine($"Parsed successfully: The transformed value is: {transformedValue}");
}
else
{
    Console.WriteLine(parseResult.Exception!.Message);
}
```

We can refactor obtaining `parseResult` using existing functional behaviour built into C#:

```csharp
var parseResult = int.TryParse(input, out int value)
    ? Result<int>.Create(value)
    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
```

And abstract it into a function.

```csharp
Result<int> ParseForInt(string? input) => int.TryParse(input, out int value)
    ? Result<int>.Create(value)
    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
```

We can then refactor:

```csharp
var input = Console.ReadLine();
var parseResult = ParseForInt(input);
//....
```

Finally we can abstract out to a string extension method:

```csharp
public static class stringExtensions
{
    public static Result<int> ParseForInt(this string? input) =>
        int.TryParse(input, out int value)
        ? Result<int>.Create(value)
        : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
}
```

And refactor:

```csharp
var parseResult = Console
    .ReadLine()
    .ParseForInt();
//....
```

We now have *fluent* style operations for getting the `int` from the console input with `null` handling.

## Working with `Result<T>`

Our console app now looks like this:

```csharp
var parseResult = Console
    .ReadLine()
    .ParseForInt();

if (parseResult.HasValue)
{
    double transformedValue = Math.Sqrt(parseResult.Value);
    transformedValue = Math.Round(transformedValue, 2);
    Console.WriteLine($"Parsed successfully: The transformed value is: {transformedValue}");
}
else
{
    Console.WriteLine(parseResult.Exception!.Message);
}
```

How do we refactor the conditional statement into *fluent* operations.

### Outputting a Result

We can abstract the console write into a generic `Output` operation on `Result<T>` like this:

```csharp
public void Output(Action<T>? hasValue = null, Action<Exception>? hasException = null)
{
    if (this.Exception is null)
        hasValue?.Invoke(_value!);
    else
        hasException?.Invoke(_exception!);
}
```

And then use it like this:

```csharp
var parseResult = Console
    .ReadLine()
    .ParseForInt()
    //  Handle the transforms
    .Output(
        hasValue: (value) => Console.WriteLine($"Parsed successfully: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failed to parse input: {ex.Message}")
    );
```

## Applying Transforms

This is where the real power comes in:

1. Chaining operations. 
2. Handling nulls and exceptions in the chain using *Railway Orientated Programming*.

The basic `Transform` pattern is:

```csharp
public  Result<TOut> ApplyTransform<TOut>( Func<T, Result<TOut>> transform)
    => this.HasValue
        ? transform(this.Value!)
        : Result<TOut>.Failure(this.Exception!);
```

A deceptively simple piece of very powerful code.  It executes the provided `tranform` only if it has Valid `Value`.  Otherwise it create a new `Result<Tout>`  and passes on the exception.  

Used in the console app:

```csharp
var parseResult = Console
    .ReadLine()
    .ParseForInt()
    .ApplyTransform((value) => Math.Sqrt(value))
    //  Handle the transforms
    .Output(
        hasValue: (value) => Console.WriteLine($"Parsed successfully: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failed to parse input: {ex.Message}")
    );
```

 In this case, `ApplyTransform` is provided with a lambda expression to calculate the square root of the input.  The compiler interprets `TOut`. 

The now complete refactored version of the console app:

```csharp
Console
    .ReadLine()
    .ParseForInt()
    .ApplyTransform((value) => Math.Sqrt(value))
    .ApplyTransform((value) => Math.Round(value, 2))
    //  Handle the transforms
    .Output(
        hasValue: (value) => Console.WriteLine($"Parsed successfully: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failed to parse input: {ex.Message}")
    );
```


If the same lambda expressions are used a lot, cache them like this:

```csharp
Result<double> SquareRoot(int value)
    => Result<double>.Create(Math.Sqrt(value));

// or

public static class Utilities
{
    public static Func<int, Result<double>> GetSquareRoot => (value)
        => Result<double>.Create(Math.Sqrt(value));
}
```

And then:

```csharp
.ApplyTransform(SquareRoot)
.ApplyTransform(Utilities.GetSquareRoot)
```

## So What Have We Learnt from this Exercise

*Monads* are wrappers/containers: just an implementation of the **Decorator Pattern** with some specific *functional* methods.  They provide high level coding functionality, abstracting underlying standard C# coded functionality. 

### Functions

In FP you will hear the phrase "Functions are first class citizens" a lot.  C# has delegates and `Func` and `Action` implementations.  In OOP you will rarely see them used.

In FP, *Functions* are methods that take an input, apply one or more transforms to that input, and produce an output.  *Functions* are the building blocks of FP.

In functional programming you apply functions to data and produce a result.  In OOP you pass data into methods to mutate the state of objects.

### Railway Orientated Programming

Whether you realised it or not, FP patterns implement *Railway Orientated Programming*.  If the input `Result<T>` is in *Exception* state, any transform short-circuits, passing the exception to the output `Result<TOut>`: the transform function is not executed.  Once on the *Exception* track, you stay there..

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

