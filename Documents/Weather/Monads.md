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
```

We can then define:

```csharp
var input = Console.ReadLine();

var result = Result<string>.Create(input);
```


## Working with `Result<T>`

Our console app now looks like this:

```csharp
string? text = Console.ReadLine();

var result = Result<string>.Return(text);
```

Next we need to handle the Monad in `Console.WriteLine`.

For this we implement a `Match` method.  A standard implementation looks like this:

```csharp
public void Match(Action<T> success, Action<Exception> failure)
{
    if (_exception is null)
        success(_value!);
    else
        failure(_exception!);
}
```

The console app now looks like this:

```csharp
string? text = Console.ReadLine();

var result = Result<string>.Return(text);

result.Match(
    success: value => Console.WriteLine($"Success: {value}"),
    failure: ex => Console.WriteLine($"Failure: {ex.Message}")
);
```

We're passing methods into match, and exectuing the appropriate method for the `Result` state.  The success path is only executed if a valid value exists.

## Chaining

At this point, interesting, a different way of coding, but there's no real savings.  The real benefits come when we start chaining things together.

We can update our console app to start chaining:

```csharp
string? text = Console.ReadLine();

Result<string>
    .Return(text)
    .Match(
        success: value => Console.WriteLine($"Success: {value}"),
        failure: ex => Console.WriteLine($"Failure: {ex.Message}")
    );
```

Great, but the real power is in `Map` and `Bind`.

## Map

`Map` merges a normal function into the Monad.  It can be represented like this:

```
(in->out) -> Monad<in> -> Monad<out>.
```

The basic `Map` in `Result<T>` looks like this:

```csharp
public Result<TOut> Map<TOut>(Func<T, TOut> func)
{
    if (_exception is not null)
        return Result<TOut>.Return(_exception);

    try
    {
        return new Result<TOut>(func(_value!));
    }
    catch (Exception ex)
    {
        return new Result<TOut>(ex);
    }
}
```

There are two paths:

- **Success** - `Func` is executed within a `try/catch` and the result returned in a new `Result<TOut>`, or any generated exception captured and wrapped in a new `Result<TOut>`.
- **Failure** - The excpetion in the input `Result<T>` is wrapped in a `Result<TOut>` and returned.

Now Consider:

```csharp
public static int Parse(string s);
```

It fits the pattern, so we can chain it into our pipeline:

```csharp
Result<string>
    .Return(input)
    .Map(int.Parse)
    .Match(
        success: value => Console.WriteLine($"Parsed successfully: The transform result of {input} is: {value}"),
        failure: ex => Console.WriteLine($"Failed to parse input: {ex.Message}"
    ));
```

Note that `Map` can switch types, so `T->TOut` and `T->T` methods are both valid.  In this case we switch from an input `string` to an output `int`.

For the next step we run into a problem While `Math.Sqrt(value)` fits the pattern, the output from `.Map(int.Parse)` is a `int`.  We solve this by changing out the parser method to `double.Parse`:

```csharp
Result<string>
    .Return(input)
    .Map(double.Parse)
    .Map(Math.Sqrt)
    .Match(
        success: value => Console.WriteLine($"Parsed successfully: The transform result of {input} is: {value}"),
        failure: ex => Console.WriteLine($"Failed to parse input: {ex.Message}"
    ));
```

Next `Math.Round(value, 2)`.  It doesn't fit the pattern.

The solution is to build a lambda expression that fits the pattern:

```csharp
    .Map(value => Math.Round(value, 2))
```

## Bind

The basic bind pattern is:

```csharp
(in->Monad(out)) -> Monad<in> -> Monad<out>.
```

The `Bind` implementation in `Result<T>`:

```csharp
public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> func)
    => _exception is null
        ? func(_value!)
        : Result<TOut>.Return(_exception!);
```

We can now write a function to round:    

```csharp
Result<double> RoundToTwoPlaces(double value)
    => Result<double>.Return(Math.Round(value, 2));
```

And the original program can now be re-written:

```csharp
Result<string>
    .Return(input)
    .Map(double.Parse)
    .Map(Math.Sqrt)
    .Bind(RoundToTwoPlaces)
    .Match(
        success: value => Console.WriteLine($"Parsed successfully: The transform result of {input} is: {value}"),
        failure: ex => Console.WriteLine($"Failed to parse input: {ex.Message}"
    ));
```

We can incorporate flexible rounding:

```csharp
Result<double> Round(double value, int places)
    => Result<double>.Return(Math.Round(value, places));
```

And:

```csharp
    .Bind(value => Round(value, 2))
```

## Monadic Extensions

While we've made the pipeline very succinct and expressive, the constructor looks a little clumsy.

```csharp
Result<string>
    .Return(input)
```

We can improve this by adding a `Map` extension method to `string` like this:

```csharp
public static Result<TOut> Map<TOut>(this string? input, Func<string, TOut> mapper)
    => Result<string>
        .Return(input)
        .Map(mapper);
```

So our final code:

```csharp
string? text = Console.ReadLine();

input
    .Map(double.Parse)
    .Map(Math.Sqrt)
    .Bind(value => Round(value, 2))
    .Match(
        success: value => Console.WriteLine($"Parsed successfully: The transform result of {input} is: {value}"),
        failure: ex => Console.WriteLine($"Failed to parse input: {ex.Message}"
    ));
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

