# Functional Programming in C#

This article provides an insight into my personal implementation of Functional Programming [FP from now on] in C# and the DotNet Framework, and my personal journey to Monad enlightenment.  
As programmers brought up on a OOP diet, Functional Programming [FP from now] is a foreign land.  This article presents my personal implementation of FP in C# and the DotNet Framework, and my journey to understanding Monads.  

It's hard to remember that first point where you saw a chink of light in the otherwise unintelligible articles about Nomands, but this description sticks in my mind, and it sums up what my implmentation is base on.

> FP is about computing a result.  When you call a FP function you pass in a value and get back a result.  There's no mutation of state, no side effects, and no changes to the input value.  The function takes the input, applies a transform, and returns a new value.

My implementation has immutable `Result<T>` and `Result` types.  They represent the result of a computation, and handle errors and exceptions in a functional way.
 
## The Elavated World

Read any literature on FP and the term *Elevated World* soon crops up.

Sounds daunting, but *Elevation* is the process of taking a normal type and elevating it to an elevated type.  My *Elevated World* is `Result<T>` and `Result`.

The simplest way to elevate a type is to use one of the static construstors.  In a simple console app you could do this:

        var result = Math.Sqrt(value);
        result = Math.Round(result, 2);
        Console.WriteLine($"Parsed successfully: The transformed value of {value} is: {result}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An exception occurred: {ex.Message}");
    }
}
else
{
    Console.WriteLine("Input cannot be null or empty.");
}
```
 
## The Elevated World

Read any literature on FP and the *Elevated World* soon appears.

It sounds complicated, but *Elevation* is simply the process of adding a wrapper around a normal type - taking a normal type and elevating it to an elevated type.  My *Elevated World* is `Result<T>` and `Result`.

You can elevate simply by using one of the static constructors.  In a simple console app:

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

We can do better: The `Create` static constructor abstracts null handling.

```csharp
var input = Console.ReadLine();
var result = Result<string>.Create(input);
```

One final step to make this code *fluent* is to extend `string`.

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
 
## Unwapping the Elevated World

How do we unwrap our Result<T>?  In Result<T> we have a `HasValue` and `HasException` property.  We can use these to determine if the operation was successful or not.

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

We're back to imperative programming.  We can go *FP* using the `Output` method to output the result.

```csharp
Console
    .ReadLine()
    .ToResult()
    .Output(
        hasValue: (value) => Console.WriteLine($"Success: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```
We *wrap* the two possible console outputs into lambda functions to pass into `Output`. `Output` calls the appropriate delegate based on the state of `Result<T>`.


## Transforming the Elevated World

The *Elevated World* is a world of immutable types.  The `Result<T>` and `Result` types are immutable, so we can't change the state of an object.  We can only create new objects.

We can transform the `Result<T>` type using the `ApplyTransform` set of methods.  The basic pattern of a transform is:

```csharp
Tin -> Function -> Result<TOut>
```

Let's go back to our console app and parsing the input.  We can wrap the parsing logic into a lambda expression.  The transform function takes the input value, applies the transformation, and returns a new `Result<T>` type.

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

The `ApplyTransform` method is a key part of the `Result<T>` type.  It allows us to apply a transformation to the value contained within the `Result<T>`.  If the `Result<T>` has a value, the transformation is applied, and a new `Result<TOut>` is returned.  If it has an exception, the exception is propagated without applying the transformation.

We can improve this because there's an `ApplyTransform` that abstracts the exception capture logic: it wraps `Tin -> Function -> TOut` in a `try/catch` logic.

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

We can now add the *SquareRoot* and *Round* transforms.

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

We can modify the parsing transform to parse directly to a `double`:

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

## Side Effects

There are times when you want to output a result, normally mutate some object state, at some point in the chain.  You can always break put of the chain and then restart, but that's clunky.

`ApplySideEffect` is designed for that purpose.  Here's a refactored version of the console app to output the initial parsing value.

```csharp
double doubleValue = 0;

Console
    .ReadLine()
    .ToResult()
    .ApplyTransform(double.Parse)
    .ApplySideEffect((value) => doubleValue = value)
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
    .ApplySideEffectAsync((value) => doubleValue = value)
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
public static Task<Result<T>> ApplySideEffectAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    => task
        .ContinueWith(CheckForTaskException)
        .ContinueWith((t) => t.Result.ApplySideEffect(hasValue, hasException));
```

For the record , the `CheckForTaskException` method is:

```csharp
private static Result<T> CheckForTaskException<T>(Task<Result<T>> task)
    => task.IsCompletedSuccessfully
        ? task.Result
        : Result<T>.Failure(t.Exception
            ?? new Exception("The Task failed to complete successfully"));
```
