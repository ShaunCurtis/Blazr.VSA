# Functional Programming in C#

Applying the Functional Programming paradigm to Object Oriented Programming languages such as C# requires compromises: there are fundimental concepts that don't sit well on the other side.

> Note: From here on Functional Programming is **FP** and Object Oriented Programming is **OOP**.  

This article describes my personal implementation on fusing FP into C# and the DotNet Framework.

## What is Functional Programming

The first question to answer is "What is the difference between FP and OOP?".

FP is about computing a result.  When you call a FP function you pass in a value and get back a result.

An OOP function performs actions.  It may do the same as a FP function, but it often changes this, adds that.  It may, or may not return a result, often a `bool` to indicate success/failure.

`Void` returns are common in OOP.  In FP, there's no such thing.

Some examples:

`String.ToUpper()` is a FP style function.  It takes in a string as an input, applies a transform, and returns new string.
 

## `Result<T>` and `Result`

Everything revolves around `Result<T>` and `Result`.  They are the foundation stones of my FP implementation,  Just as `Task` and `Task<T>` are to async coding.

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

There's a separate article that covers `Result<T>` and `Result` in detail, so I'll only cover the basics here.

A result has two possible states:

- **Success**: The operation completed successfully.
- **Failure**: The operation failed, and the result contains an `Exception` or an error message wrapped in a `ResultException`. 

The basic definitions are as follows:

```csharp
public record Result<T>
{
    private readonly Exception? _exception;
    private readonly T? _value;
    private ResultException _defaultException => new ResultException("An error occurred. No specific exception provided.");

    private Result(T? value) 
        => _value = value;

    private Result(Exception? exception) 
        => _exception = exception ?? _defaultException;

    private Result() 
        => _exception = _defaultException;

    public static Result<T> Create(T? value) => 
        value is null
            ? new(new ResultException("T was null."))
            : new(value);

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Exception exception) => new(exception);

    public static Result<T> Failure(string message) => new(new ResultException(message));

    //... functional methods
}
```

```csharp
public record Result
{
    private readonly Exception? _exception;

    private Result(Exception? exception)
        => _exception = exception
            ?? new ResultException("An error occurred. No specific exception provided.");

    private Result() { }

    public static Result Success() => new();
    public static Result Failure(Exception? exception) => new(exception);
    public static Result Failure(string message) => new(new ResultException(message));

    //... functional methods
}
```

The real power in Result comes when we add FP methods.  Before we start on those, we need to cover some more basics.

## Expressions and Statements

Consider this code:

```csharp
int x = 0;

if(someBoolCondition is true)
    x = 2;
else
    x = 3;
```

`int x = 0;` is an **expression**, but it only sets a default value.  The real assignment is in the `if.. else..` **statement** block.

Modern C# has added this *syntatic sugar*:

```csharp
int x = somecondition is true ? 2 : 3;
```

But this only works for simply one liners.

This doesn't compile:

```csharp
int x = someBoolCondition is true
    ? { return 2;} 
    : { return 3;};
```

### Boolean Extensions

We can resolve this problem by adding some FP to `bool`:

You could create a True/False mapper like this:

```csharp
public static T Map<T>(this bool value, Func<T> isTrue, Func<T> isFalse)
    => value ? isTrue() : isFalse();
```

And use it like this:

```csharp
int x = someBoolCondition.Map(
    isTrue: 
        {
            // Do some work
            return 2;
        },
    isFalse: 
        {
            // Do some work
            return 3;
        }
);
```

However, in my FP world all functions return a result, so `Map` looks like this:

```csharp
public static Result<T> Map<T>(this bool value, Func<bool, Result<T>> mapping)
    => mapping(value);
```

Which is used like this:

```csharp
Result<int> x = someBoolCondition.Map((value) => value? 1 : 0);
```

We can also provide a second map:

```csharp
public static Result<T> Map<T>(this bool value, Func<Result<T>> isTrue, Func<Result<T>>? isFalse)
{
    if (value)
        return isTrue();

    if(!value && isFalse != null)
        return isFalse();

    return Result<T>.ReturnException("The bound bool was false");
}
```

Which is used in a variety of ways.  Examples:

```csharp
Result<int> x = someBoolCondition.Map(isTrue: 1);

Result<int> x = someBoolCondition.Map(isTrue: 1, isFalse 0);
```

## Result Mapping

Mapping is the process of applying a transform to the value of a Result.

There are four basic transforms we can apply:

 - Map a `Result<T>`to a new `Result<T>`, where `T` is the same type on both.  We can express this as `T -> result<T>`.
 - Map a `Result<T>` to a `Result<TOut>`, where `TOut` is a different type to `T`.  We can express this as `T -> Result<TOut>`.  
  - Map a `Result<T>` to a `Result`.  We can express this as `T -> Result`. 
  - Map a `T => TOut` to a `Result<TOut>`. 

The following `Map` function covers the first two transforms.


```csharp
    public Result<TOut> Map<TOut>(Func<T, Result<TOut>> success, Func<Exception, Result<TOut>>? failure = null)
    {
        if (_exception is null)
            return success(_value!);

        if (_exception is not null && failure != null)
            return failure(_exception!);

        return Result<TOut>.Failure(_exception!);
    }
```

And this the third:

```csharp
public Result Map(Func<T, Result>? mapping = null)
{
    if (_value is not null && mapping != null)
        return mapping(_value!);

    if (_value is not null)
        return Result.Success();

    return Result.Failure(_exception ?? _defaultException);
}
```

And finally the fourth:

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