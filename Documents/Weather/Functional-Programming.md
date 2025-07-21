# Functional Programming in C#

> Throughout this article, I use **FP** for Functional Programming and **OOP** for Object Oriented Programming.  

This article describes my personal implementation of FP into C# and the DotNet Framework.

There are plenty of articles on the Internet explaining what FP is, and any number about Monads.

My implementation is based on the following definition:

> FP is fundimentally about computing a result.  When you call a FP function you pass in a value and get back a result.  There's no mutation of state, no side effects, and no changes to the input value.  The function takes the input, applies a transform, and returns a new value.

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

There's a separate article that delves into the implementation detail of `Result<T>` and `Result`, I'll only cover the basics here.

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

There are several static constructors:

```csharp
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Exception exception) => new(exception);
    public static Result<T> NoValueFailure() => new(new ResultException("No value was returned"));
    public static Result<T> Failure(string message) => new(new ResultException(message));
```

## Fundimental Result Operations

#### Dealing with a Null Input

The most common code pattern in C# is the null check.

```csharp
MyClass? x;

if(x is null)
    //do something;
else
    // do something else;
```

In the Result world, the `Create` static constructor deals with the null case for you.

```
public static Result<T> Create(T? value) => 
    value is null
        ? new(new ResultException("T was null."))
        : new(value);
```

## Result Mapping

Mapping is the process of applying a transform to the input and producing a Result output.

The basic template is:

```csharp
public Result<TOut> MapToResult<TOut>(Func<T, Result<TOut>> success)
{
    if (_exception is null)
        return success(_value!);

    return Result<TOut>.Failure(_exception!);
}
```

It may look simple, but it's a very powerful piee of code.



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