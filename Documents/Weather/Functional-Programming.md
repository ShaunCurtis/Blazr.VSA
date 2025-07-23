# Functional Programming in C#

> Throughout this article, I use **FP** for Functional Programming and **OOP** for Object Oriented Programming.  

This article describes my personal implementation of FP into C# and the DotNet Framework.

There are plenty of articles on the Internet explaining what FP is, and any number about Monads.

My implementation is based on the following definition:

> FP is about computing a result.  When you call a FP function you pass in a value and get back a result.  There's no mutation of state, no side effects, and no changes to the input value.  The function takes the input, applies a transform, and returns a new value.

## `Result<T>` and `Result`

If you've read any amount of literature on FP you'll have come across the *Elevated World*.

My *Elevated World* is the `Result<T>` and `Result` types..

Any method that would return a value:

```csharp
public int ToInt(string value) {..}
```

returns a `Result<T>`:
 
```csharp
public Result<int> ToInt(string value) {..}
```

Any method that would return a `void`:

```csharp
public void DoSomething(string value) {..}
```

returns a `Result`:

```csharp
public Result DoSomething(string value) {..}
```

There's a separate article on the implementation detail of `Result<T>` and `Result`, I'll only cover the basics here.

A result has two states:

- **Success**: The operation completed successfully.
- **Failure**: The operation failed, and the result contains an `Exception` or an error message wrapped in a `ResultException`. 

Result is a record type: it's immutable.

Internally a `Result<T>` has:

```csharp
    private readonly Exception? _exception;
    private readonly T? _value;
```

And `Result` has: 

```csharp
    private readonly Exception? _exception;
```

If the operation was successful, `_exception` is `null` and `_value` contains the result value. If the operation failed, `_exception` contains the exception that caused the failure.

## Fundimental Result Operations

### Elevation

Elevation is the process of elevating a normal type to an elevated type.

The simplest way is to use one of the `Result<T>` static constructors.  

> Note that there are no public *ctors*: you must use the static constructors.

```csharp
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Exception exception) => new(exception);
    public static Result<T> Failure(string message) => new(new ResultException(message));
```

#### Dealing with Nullable Inputs

The most common code pattern in the C# book is the null check.

```csharp
MyClass? x;

if(x is null)
    //do something;
else
    // do something else;
```

The `Create` static constructor deals with the null case for you.

```
public static Result<T> Create(T? value) => 
    value is null
        ? new(new ResultException("T was null."))
        : new(value);
```

#### Monadic Functions

Monadic functions are delegates with the following basic pattern:

```csharp
Func<T, Result<TOut>>
```

They take a normal type in and return an elevated type.

Here's a simple example:

```csharp
public Result<int> ParseForInt(string input)
{
    var intResult  = int.Parse(input);

    return Result<int>(intResult);
}
```

Which we can rewrite as:

```csharp
public Func<string, Result<int>> ParseForInt => (string input) =>
{
    var intResult = int.Parse(input);
    return Result<int>.Create(intResult);
};
```

### Lowering

Lowering is the process of outputting a normal type from an elevated type.

```csharp
public void OutputResult(Action<T>? success = null, Action<Exception>? failure = null)
{
    if (_exception is null)
        success?.Invoke(_value!);
    else
        failure?.Invoke(_exception!);
}
```

Here's a simple example to demonstrate it usage:

```csharp
value = null;

Result<string>
    // elevates value
    .Create(value)
    // lowers Result<string>
    .OutputResult(
        success: (value) => Console.WriteLine($"Success: {value}"),
        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );
```

### Result Mapping

Mapping is the process of applying a transform to the input and producing a Result output.

The basic template is:

```csharp
public Result<TOut> ApplyTransform<TOut>(Func<T, Result<TOut>> success)
{
    if (_exception is null)
        return success(_value!);

    return Result<TOut>.Failure(_exception!);
}
```

It may look simple, but it's a very powerful piece of code.

Here's a simple example in a console app:

```csharp
string? value = Console.ReadLine();

// monadic function
ParseForInt(value)
    // Applying a Mapping function
    .ApplyTransform<double>(SquareRoot)
    // Output the result
    .OutputResult(
        success: (value) => Console.WriteLine($"Success: {value}"),
        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );

Result<double> SquareRoot(int value)
    => Result<double>.Create(Math.Sqrt(value));

Result<int> ParseForInt(string? input)
    => int.TryParse(input, out int result)
        ? Result<int>.Create(result)
        : Result<int>.Failure(new FormatException("Input is not a valid integer."));
```

Try entering different type of input.  `<CTL>Z` will enter a `null`.

1. `ParseForInt` uses the [horrible] `TryParse` method to return either a success or failure `Result<int>`.
1. `ApplyTransform` then applies `SquareRoot` to the value of the input `Result<T>` if it's in success state.  If it's in failure, it creates and returns a new `Result<double>` with the `Exception` from the input Result<int>.
1. Finally `OutputResult` outputs to the console based on the result state.

What you see is:

1. The ability to chain simple functions together.
2. The ability to compose complex functions by combining simple functions.
3. An implementation of `Railway Orientated Programme` to handle errors and exceptions.

There are four common transforms:

 1. Map a `Result<T>`to a new `Result<T>`, where `T` is the same type on both.  We can express this as `T -> result<T>`.
 2. Map a `Result<T>` to a `Result<TOut>`, where `TOut` is a different type to `T`.  We can express this as `T -> Result<TOut>`.  
 3. Map a `Result<T>` to a `Result`.  We can express this as `T -> Result`. 
 4. Map a `T => TOut` to a `Result<TOut>`. 

The first two are handled by the basic template above.

The third by:

```csharp
public Result ApplyTransform(Func<T, Result>? mapping = null)
{
    if (_exception is null && mapping != null)
        return mapping(_value!);

    if (_value is not null)
        return Result.Success();

    return Result.Failure(_exception ?? _defaultException);
}
```

And the fourth by:

```csharp
public Result<U> Map<U>(Func<T, U> mapping)
{
    if (_exception is not null)
        return Result<U>.Failure(_exception!);

    try
    {
        return Result<U>.Create(mapping(_value!));
    }
    catch (Exception ex)
    {
        return Result<U>.Failure(ex);
    }
}
```

And that is it.  But it isn't, because we need to deal with `async` functions and `Task`.

### Side Effects

There are times when using FP in a complex object setting where you will need to update the object state.  You can do it within a `ApplyTransform` lambda expression, but that's messy.

`Result<T>` has a set of `ApplySideEffect` methods so you can be explicit.  The basic pasttern is:

```csharp
public Result<T> ApplySideEffect(Action<T>? success = null, Action<Exception>? failure = null)
{
    if (_exception is null)
        success?.Invoke(_value!);
    else
        failure?.Invoke(_exception!);

    return this;
}
```

And in use:

```csharp
int intValue;

ParseForInt(value)
    // Get out an intermediate result
    .ApplySideEffect(success: (value) => Console.WriteLine($"Parsed value: {value}"))
    // Applying a Mapping function
    .ApplyTransform(Utilities.ToSquareRoot)
    // Output the result
    .OutputResult(
        success: (value) => Console.WriteLine($"Success: {value}"),
        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );
```



## Task<Result<T>> and Task<Result>

If we do an async operation in a monadic function we have to deal with the `Task` wrapper around the result.

For the purposes of this discussion I've made `ParseForInt` and async method:

```csharp
async Task<Result<int>> ParseForIntAsync(string? input)
{ 
    await Task.Yield(); // Simulate async operation
    return int.TryParse(input, out int result)
        ? Result<int>.Create(result)
        : Result<int>.Failure(new FormatException("Input is not a valid integer."));
}
```

The return type is now a `Task<Result<int>>`, so we need to add some FP methods to `Task<Result<T>>`.

### Map

First is a mapper:

```csharp
public static async Task<Result<TOut>> MapTaskToResultAsync<T, TOut>(this Task<Result<T>> task, Func<T, Result<TOut>> mapping)
{
    var result = await task.HandleTaskCompletionAsync();
    return result.ApplyTransform<TOut>(mapping);
}
```

It awaits the Task and then executes the synchronous `mapping` function.  The return is a `Result<TOut>` wrapped in another `Task`.

### Output

And the outputter:

```csharp
public static async Task OutputTaskAsync<T>(this Task<Result<T>> task, Action<T>? success = null, Action<Exception>? failure = null)
    => await task.HandleTaskCompletionAsync()
        .ContinueWith((t) => t.Result.OutputResult(success: success, failure: failure));
```

The console app:

```csharp
string? value = Console.ReadLine();

await ParseForIntAsync(value)
    // Applying a Mapping function
    .MapTaskToResultAsync(Utilities.ToSquareRoot)
    // Output the result
    .OutputTaskAsync(
        success: (value) => Console.WriteLine($"Success: {value}"),
        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );
```

### Side Effects

And side effects:

```csharp
public static Task<Result<T>> TaskSideEffectAsync<T>(this Task<Result<T>> task, Action<T>? success = null, Action<Exception>? failure = null)
    => task.HandleTaskCompletionAsync()
        .ContinueWith((t) => t.Result.ApplySideEffect(success, failure));
```

The console app:

```csharp
string? value = Console.ReadLine();

// monadic function
await ParseForIntAsync(value)
    .TaskSideEffectAsync(success: (value) => Console.WriteLine($"Parsed value: {value}"))
    // Applying a Mapping function
    .MapTaskToResultAsync(Utilities.ToSquareRoot)
    // Output the result
    .OutputTaskAsync(
        success: (value) => Console.WriteLine($"Success: {value}"),
        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
    );
```

