# Results

Consider this classic pattern:

```csharp
private async ValueTask<TRecord?> ExecuteQueryAsync<TRecord>(QueryRequest request)
```

We return either an instance of `TRecord` or null.

It smells.  What does `null` mean?  

1. No record exists with that Id.
2. The API call timed out.
3. Your request was bad.
4. Bog off, I'm busy.

Unless you're putting the information somewhere else for the caller to retrieve, you don't know.  You can raise various exceptions to trap, but that's expensive and excessive.

The answer is to return a result object.  

First a very general base result to handle status information only.

```csharp
public readonly record struct Result
{
    private Exception? _error { get; init; }
    
    private Result(Exception error)
    {
        IsSuccess = false;
        _error = error;
    }

    public bool IsSuccess { get; init; } = true;
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Returns true is failure and sets the out item to the exception
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public bool HasFailed([NotNullWhen(true)] out Exception? exception)
    {
        if (this.IsFailure)
            exception = _error;
        else
            exception = default;

        return this.IsFailure;
    }

    /// <summary>
    /// Converts the Result to a UI DataResult
    /// </summary>
    public IDataResult ToDataResult => new DataResult() { Message = _error?.Message, Successful = this.IsSuccess };

    public static Result Success() => new() { IsSuccess = true };
    public static Result Fail(Exception error) => new(error);
}
```

And an implementation:

```csharp
public sealed record DataResult : IDataResult
{ 
    public bool Successful { get; init; }
    public string? Message { get; init; }

    internal DataResult() { }

    public static DataResult Success(string? message = null)
        => new DataResult { Successful = true, Message= message };

    public static DataResult Failure(string message)
        => new DataResult { Message = message};

    public static DataResult Create(bool success, string? message = null)
        => new DataResult { Successful = success, Message = message };
}
```

Next we can add a result:

```csharp
public sealed record DataResult<TData> : IDataResult
{
    public TData? Item { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    internal DataResult() { }

    public static DataResult<TData> Success(TData Item, string? message = null)
        => new DataResult<TData> { Successful = true, Item = Item, Message = message };

    public static DataResult<TData> Failure(string message)
        => new DataResult<TData> { Message = message };
}
```

## Coming Soon

Net 9 will hopefully change how we handle returns though a new launuage feature called *Discriminated Unions*.  Search for *Discriminated Unions* for more information.  