# Functional Programming in C#

Programmers brought up on a strictly OOP diet find the whole concept of Functional Programming [FP from now] very alien.

This article takes a very simple console application and walks through refactoring it using FP principles.  Along the way, I'll gently introduce you to Monads.  

It's hard to remember that first point where I saw a chink of light in the otherwise baffling articles about Nomads, but this description sticks in my mind.

> FP is about computing a result.  When you call a FP function you pass in a value and get back a result.  There's no mutation of state, no side effects, and no changes to the input value.  The function takes the input, applies a transform, and returns a new value.

Looking back, I think it was the terminology that was most confusing.  In OOP many of the terms used have somewhat different meanings.

My implementation of FP is a library called *Blazr.Manganese*. It's based around two immutable types: `Result<T>` and `Result`.  They represent the result of a computation, and handle errors and exceptions in a functional way.  I use what I consider to be clearer, more descriptive terminology for the various FP patterns.  There's no `Return`, `Map`, `Bind` or `Match` methods in `Result<T>`.
 
## The Elevated World

Read any literature on FP and the *Elevated World* soon appears.

It sounds complicated, but *Elevation* is simply the process of adding a wrapper around a normal type - taking a normal type and elevating it to an elevated type.  `Result<T>` and `Result` are my *elevated world*.

You can elevate simply by using one of the static constructors:

```csharp
var input = Console.ReadLine();

Result<string> result;

if (input is not null)
{
    result = Result<string>.Success(input);
}
else
{
    result = Rwsult<string>.Failure(new ResultException("Input was null."));
}
```

*Old school* C#.  The conditional statements are pretty verbose. Modern C# has a terser short form of `if`:

```csharp
var input = Console.ReadLine();

Result<string> result = input is not null
    ? Result<string>.Success(input)
    : Result<string>.Failure(new ResultException("Input was null."));
```

The imperative statement based style has changed to an expression based style.  The short form is an example of where FP styled syntax has crept into C#.

Note: This is just *syntactic sugar*.  The `if` statement is still there when the code is *lowered* by the compiler: we just don't need to worry about it.  The language, and our own higher level code, abstracts it.

We can do better: Result's `Create` static constructor abstracts null handling.

```csharp
public static Result<T> Create(T? value) =>
    value is null
        ? new(new ResultException("T was null."))
        : new(value);
```

So we now have:

```csharp
var input = Console.ReadLine();
var result = Result<string>.Create(input);
```

The final step to make this code *fluent* by extending `string`.

```csharp
public static class stringExtensions
{
    public static Result<int> ToResult(this string? input) =>
        Result<string>Create(value);
}
```

We can now write this:

```csharp
var result = Console
    .ReadLine()
    .ToResult();
```

### Pertinent FP Notes

FP doesn't have imperative statements like `if` or `else`.  It is expression based, there is always a result to any transaction.  `Result<T>` is stateful: it contains either result, based on state.

The process of *elevating* a type is the first step in FP.  We take a normal type, like `string`, and elevate it to a `Result<string>`.  This allows us to handle errors and exceptions in a functional way.  

In classic FP, you call `Return` on the Monad to wrap it.  `Result<T>` has `Create`, `Success` and `Failure` that return the provided value or exception in a `Result<T>`.

## Unwrapping the Elevated World

We have a `Result<T>`, how do we unwrap it: for example, output it to the console?

Result<T> exposes four properties:

```csharp
    public readonly Exception? Exception;
    public readonly T? Value;
    public bool HasException => Exception is not null;
    public bool HasValue => Exception is null;
```

These are `public` so `Result<T>` can be used in *OOP*:

```csharp
var result = Console
    .ReadLine()
    .ToResult();

if (result.HasValue)
{
    //...
    Console.WriteLine($"Parsed successfully: The transformed value is: {result.Value}");
}
else
{
    Console.WriteLine($"Failed to parse input: {result.Exception!.Message}");
}
```

In `Result` that code block gets wrapped into a generic `Output` method.

```csharp
public Result<T> Output(Action<T>? hasValue = null, Action<Exception>? hasException = null)
{
    if (HasValue)
        hasValue?.Invoke(Value!);
    else
        hasException?.Invoke(Exception!);

    return this;
}
```

So we can write this by wrapping the console outputs in lambda expressions:

```csharp
Console
    .ReadLine()
    .ToResult()
    .Output(
        hasValue: (value) => Console.WriteLine($"Success: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

An alternative is to output the message directly.  `Result<T>` has the following method:

```csharp
public TOut OutputValue<TOut>(Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
    => this.HasValue
    ? hasValue.Invoke(this.Value!)
    : hasException.Invoke(Exception!);
```

Which we can use like this:

```csharp
Console.WriteLine(Console
        .ReadLine()
        .ToResult()
        .OutputValue<string>(
            hasValue: (value) => $"Success: The transformed value is: {value}",
            hasException: (ex) => $"Failure: {ex.Message}"
        )
    );
```

### Pertinent FP Notes

In a pure FP implementation, the properties would be private.  There's no valid FP reason to access them directly. Making them public is intentional:  make `Result<T>` usable in both *OOP* and *FP*.


## Transforming the Elevated World

So far, we've elevated a type, unwrapped it, and output the result to the console.  But we haven't done anything with the value.  The real power in monads comes in their ability to selectively transform the input value into an output and maintain the `Result` wrapper. The *transform* is only executed if `Result<T>` has a value.  Otherwise the method short-circuits and the exception is propagated.

`Result<T>` has the `ApplyTransform` set of methods.  The basic pattern for a transform is:

```csharp
Tin -> Apply Function -> Result<TOut>
```

The basic implementation that applies this pattern is:

```csharp
public Result<TOut> ApplyTransform<TOut>(Func<T, Result<TOut>> transform)
    => this.HasValue
        ? transform(this.Value!)
        : Result<TOut>.Failure(this.Exception!);
```

Back to the console app.  The parsing logic can be wrapped in a lambda expression.  The transform function takes the input value, applies the transformation, and returns a new `Result<T>` type.

```csharp
Console
    .ReadLine()
    .ToResult()
    .ApplyTransform((input) =>
    {
        if (!string.IsNullOrEmpty(input))
        {
            try
            {
                var value = int.Parse(input!);
                return Result<int>.Create((int)value);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure(ex);
            }
        }
        return Result<int>.Failure("Input cannot be null or empty.");
    })
    .Output(
        hasValue: (value) => Console.WriteLine($"Success: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

It captures and handles both exceptions and nulls.

The `Tin -> Apply Function -> TOut` is common enough that `Result<T>` has a `Func<T, TOut>` overload of `ApplyTransform` that wraps the `try/catch` logic for you.

```csharp
public Result<TOut> ApplyTransform<TOut>(Func<T, TOut> transform)
{
    if (this.Exception is not null)
        return Result<TOut>.Failure(this.Exception!);

    try
    {
        var value = transform.Invoke(this.Value!);
        return (value is null)
            ? Result<TOut>.Failure(new ResultException("The transfrom function returned a null value."))
            : Result<TOut>.Create(value);
    }
    catch (Exception ex)
    {
        return Result<TOut>.Failure(ex);
    }
}
```

`int.Parse` fits the delegate pattern, so can be written like this:

```csharp
Console
    .ReadLine()
    .ToResult()
    .ApplyTransform(int.Parse)
    .Output(
        hasValue: (value) => Console.WriteLine($"Success: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

*SquareRoot* and *Round* don't,  but that's not a problem as we can wrap then in lambda expressions that do.

```csharp
Console
    .ReadLine()
    .ToResult()
    .ApplyTransform(int.Parse)
    .ApplyTransform((value) => Math.Sqrt(value))
    .ApplyTransform((value) => Math.Round(value, 2))
    .Output(
        hasValue: (value) => Console.WriteLine($"Success: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

Finally, the logic can be tweaked to parse directly to a `double`:

```csharp
Console
    .ReadLine()
    .ToResult()
    .ApplyTransform(double.Parse)
    .ApplyTransform(Math.Sqrt)
    .ApplyTransform((value) => Math.Round(value, 2))
    .Output(
        hasValue: (value) => Console.WriteLine($"Success: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

We can do a little more: `Result` and `Result<T>` has a static `Result.CreateFromFunction` method:

```csharp
public static Result<TOut> CreateFromFunction<TOut>(Func<TOut> function)
    => Result.Success().ApplyTransform(function);
```

`Console.ReadLine` matches the `Func<TOut>` pattern, so:

```csharp
Console.WriteLine(
    Result<string>
        .CreateFromFunction(Console.ReadLine)
        .OutputValue<string>(
            hasValue: (value) => $"Success: The transformed value is: {value}",
            hasException: (ex) => $"Failure: {ex.Message}"
        )
    );
```

### Pertinent FP Notes

The *Elevated World* is a world of immutable types.  The `Result<T>` and `Result` types are immutable, so we can't change the state of an object.  We can only create new objects.

Functions, delegates in C#, are an integral part of FP.  They are *first class citizens* in FP, unlike in OOP C# where you will only see them rarely.  The generic versions, `Func` and `Action` are used everywhere.  Make sure you are very familiar with them.

The `Tin -> Apply Function -> TOut` delegate pattern, one input transformed to one output, is a fundamental pattern to FP: the staple diet of Monads.

This starts to drive the way you write code.  You start to think in terms of transformations, rather than statements.  Imperative style of programming is replaced with declarative style.  Methods look like this:

```csharp
public Result<T> ApplyTransform<T>(xxxRequest request);
```
or 

```csharp
public T ApplyTransform<T>(xxxRequest request);
```

If you've used the *Mediator* pattern , you'll be familiar with this style of programming.  The *Mediator* pattern is a form of FP, where the *Mediator* is the *Monad* and the request is the input value.

## Side Effects

There are times when you want to output a result, normally mutate some object state, at some point in the chain.  You can always break put of the chain and then restart, but that's clunky.

`UpdateState` is designed for that purpose.  Here's a refactored version of the console app to output the initial parsing value.

```csharp
double doubleValue = 0;

Console
    .ReadLine()
    .ToResult()
    .ApplyTransform(double.Parse)
    .UpdateState((value) => doubleValue = value)
    .ApplyTransform(Math.Sqrt)
    .ApplyTransform((value) => Math.Round(value, 2))
    .Output(
        hasValue: (value) => Console.WriteLine($"Success: The transformed value of {doubleValue} is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

## The Async Task based Elevated World

Applying asynchronous operations in FP makes things a little more complex.  The first an async operation.

This encapsulates the *ToDouble* functionality in a (fake) async Function.

```csharp
public static class Utils
{
    public static Func<string?, Task<Result<double>>> StringToDouble = async (input)
        =>
    {
        await Task.Yield();
        return double.TryParse(input, out double value)
        ? Result<double>.Create(value)
        : Result<double>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    };
}
```

I could also have written it like this:

```csharp
public static async Task<Result<double>> ParseForDouble(this string? input)
{
    await Task.Yield();
    return double.TryParse(input, out double value)
    ? Result<double>.Create(value)
    : Result<double>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
}
```

We can now use this in our console app:

```csharp
double doubleValue = 0;

await Console
    .ReadLine()
    .ToResult()
    // this is our async transition
    .ApplyTransformAsync(Utils.StringToDoubleAsync)
    // all if these operations are now continuations
    .MutateStateAsync((value) => doubleValue = value)
    .ApplyTransformAsync(Math.Sqrt)
    .ApplyTransformAsync((value) => Math.Round(value, 2))
    .OutputAsync(
        hasValue: (value) => Console.WriteLine($"Parsed successfully: The transformed value of {doubleValue} is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failed: {ex.Message}")
    );
```

It's important to observe that `.ApplyTransformAsync(Utils.StringToDoubleAsync)` returns a `Task<Result<double>>`.  All operations beyond that point are continuations of the `Task<Result<double>>`, and the methods called are extension methods on `Task<Result<T>>` not `Result<T>`.

The initial `ApplyTransformAsync` on `Result<T>` is:

```csharp
public async  Task<Result<TOut>> ApplyTransformAsync<TOut>( Func<T, Task<Result<TOut>>> transform)
    => this.HasException
        ? Result<TOut>.Failure(this.Exception!)
        : await transform(this.Value!);
```

And the specific <Task<Result<T>>> extension methods used are:

```csharp
public static Task<Result<TOut>> ApplyTransformAsync<T, TOut>(this Task<Result<T>> task, Func<T, TOut> transform)
    => task
        .ContinueWith(CheckForTaskException)
        .ContinueWith((t) => t.Result.ApplyTransform<TOut>(transform));
```

```csharp
public static Task OutputAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    => task
        .ContinueWith(CheckForTaskException)
        .ContinueWith((t) => t.Result.Output(hasValue: hasValue, hasException: hasException));
```

```csharp
public static Task<Result<T>> MutateStateAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    => task
        .ContinueWith(CheckForTaskException)
        .ContinueWith((t) => t.Result.UpdateState(hasValue, hasException));
```

For the record , the `CheckForTaskException` method is:

```csharp
private static Result<T> CheckForTaskException<T>(Task<Result<T>> task)
    => task.IsCompletedSuccessfully
        ? task.Result
        : Result<T>.Failure(t.Exception
            ?? new Exception("The Task failed to complete successfully"));
```
