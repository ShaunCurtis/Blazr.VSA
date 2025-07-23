# Result

All actions in **Diode** return a result.

Any method that returns a `T`, returns a `Result<T>` and any method that returns a `void`, returns a `Result`.

A result has two possible states:
- **Success**: The operation completed successfully
- **Failure**: The operation failed, and the result contains an error message. 

Result implements *Railway-Orientated Programming*.  Any error/exception flows up the chain bypassing executing any of steps beyond the exception/error source.

## `Result<T>`

We can define a result as follows:

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
}
```

Everything is private to provide strict control over usage.

### Constructors

There are four static constructors:

```csharp
public static Result<T> Create(T? value) => 
    value is null
        ? new(new ResultException("T was null."))
        : new(value);

public static Result<T> Success(T value) => new(value);

public static Result<T> Failure(Exception exception) => new(exception);

public static Result<T> Failure(string message) => new(new ResultException(message));
```

### Output

The result is accessed through an `Output` method.

```csharp

    public void Output(Action<T>? success = null, Action<Exception>? failure = null)
    {
        if (_exception is null && success != null)
            success(_value!);

        if (_exception is not null && failure != null)
            failure(_exception!);
    }
```

Note that this method returns a `void`.  It's an endpoint.

Demo Code:

```csharp
string? value = "Hello Result";

Result<string>.Create(value)
    .Output(
        success: (v) => Console.WriteLine($"Success: {v}"),
        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );

Result<string>.Create(value)
    .Output(
        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );

value = null;

Result<string>.Create(value)
    .Output(
        success: (v) => Console.WriteLine($"Success: {v}"),
        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

### Mapping

Mapping is the process of applying a transform to the value of a `Result<T>`.

It's important to note that mappings only execute the provided function if the source result state is success i.e. it has a valid `T` value.  If the source result is in failure state, it either passes the source result as the output result, or passes a new result (with the source exception) in failure state. 

There are three basic transforms we can apply.

#### Map a `Result<T>`to a `Result<TOut>` 

There are two variations.  The first maps to a different type:

```csharp
Result<T> => T -> TOut 
```

And the second returns the same type (but not necessarily the same value or object):

```csharp
Result<T> => T -> T 
```

Both can be accomodated with the same method:

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

This is a typical usage using a lambda expression for the mapping function:

```csharp
string? value = "Hello Result";

Result<string>.Create(value)
    .Map((v) => Result<string>.Create(v.ToUpper()))
    .Output(
        success: (v) => Console.WriteLine($"Success: {v}"),
        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

Or this where we use a function:

```csharp
private Result<string> ToUpper(string value)
    => string.IsNullOrEmpty(value)
        ? Result<string>.Failure("Value cannot be null or empty")
        : Result<string>.Create(value.ToUpper());

Result<string>.Create(value)
    .Map(ToUpper)
    .Output(
        success: (v) => Console.WriteLine($"Success: {v}"),
        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
```

#### Map a `T => TOut` function to a `Result<TOut>`

Here the passed in function only returns a `TOut`, not a `Result<T>`.  We therefore need to check the validity of the output, catch any exceptions, and where necessary return a failure.

```csharp
    public Result<TOut> Map<TOut>(Func<T, TOut> mapping)
    {
        if (_exception is not null)
            return Result<TOut>.Failure(_exception!);

        try
        {
            var result = mapping.Invoke(_value!);
            if (result is null)
                return Result<TOut>.Failure(new ResultException("The mapping function returned a null value."));
            
            return Result<TOut>.Create(mapping(_value!));
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex);
        }
    }
```

#### Map a `Result<T>` to a `Result` 

In certain situations, such as surfacing an error in UI components, all your interesting in is whether you need to display a warning/error message - a `Result` rather than a `Result<T>`.

The following code does just that.

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

An exampke of it's usage.

```csharp
string? value = null;

var result = Result<string>.Create(value)
  .Map(ToUpper)
  .Map();

DisplayError(result);

void DisplayError(Result result)
{
    result.Output(
        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
}
```

### Async Mapping

The following method handles async mapping operations:

```csharp
public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<Result<TOut>>> success, Func<Exception, Task<Result<TOut>>>? failure = null)
{
    if (_value is not null && success != null)
        return await success(_value!);

    if (_exception is not null && failure != null)
        return await failure(_exception!);

    return Result<TOut>.Failure(_exception ?? _defaultException);
}
```

### Side Effects

Side Effects provide a mechanism to effect mutate data within edit and aggregate objects.

```csharp
public Result<T> ApplySideEffect(Action<T>? success = null, Action<Exception>? failure = null)
{
    if (_value is not null && success != null)
        success(_value!);

    if (_exception is not null && failure != null)
        failure(_exception!);

    return this;
}
```


### `Result<T>` in Action

This is the Mediator `WeatherForecastRecordHandler`.  It passes a `RecordQueryRequest<DvoWeatherForecast>` object into the data pipeline and gets a `Result<DvoWeatherForecast>` returned.

```csharp
public async Task<Result<DmoWeatherForecast>> HandleAsync(WeatherForecastRecordRequest request, CancellationToken cancellationToken)
{
    var asyncResult = await _factory.CreateDbContext()
        .GetRecordAsync<DvoWeatherForecast>(new RecordQueryRequest<DvoWeatherForecast>(item => item.WeatherForecastID == request.Id.Value));
  
    return asyncResult.Bind<DmoWeatherForecast>(WeatherForecastMap.Map);
}
```

At this point, we need to convert the `DvoWeatherForecast` into a `DmoWeatherForecast`.

We use a `Bind` method to apply the mapping function.

Bind is a standard functional programming method that applies a function to the value contained in the result, and in the process transitions from a `Rrsult<T>` to a `Result<U>`.

If:
 - The source `Result<T>` is a failure, it returns the failure without applying the function.
 - The source `Result<T>` is a success, it attempts to execute `func` on the source result T value and return the result of `func` as a `Result<U>`.  The try is *defensive* coding to capture any possible exceptions.

```csharp
public Result<U> Bind<U>(Func<T, U> func)
{
    if (_exception is not null)
        return Result<U>.Return(_exception!);

    try
    {
        return Result<U>.Return(func(_value!));
    }
    catch (Exception ex)
    {
        return Result<U>.Return(ex);
    }
}
```

Basically, `Bind` is a way to chain operations on the result, allowing us to transform the value from `T` to `U` by applying the provided *T -> U* method while preserving the result's success or failure state.

