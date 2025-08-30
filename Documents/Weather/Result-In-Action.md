# Result In Action

This article shows `Result` in action and how the *Railway Orientated Programming* pattern is implemented and works.

`Result` has two tracks or states:

 - **Success**
 - **Failure**

Once a `Result` is on the Failure track any derived results are also on the failure track.

This is Map:

```csharp
public Result<TOut> ExecuteFunction<TOut>(Func<T, Result<TOut>> success)
{
    if (_exception is null)
        return success(_value!);

    return Result<TOut>.Failure(_exception!);
}
```

It only executes `success` if there's no exception [it's on the success track].  Otherwise it takes the existing exception and wraps it in a `Result<TOut>`.

We examine the following function - `GetRecordItemAsync` from the `ReadUiBroker`in detail.  

```csharp
private async Task<Result> GetRecordItemAsync(TKey id)
    => await Result<TKey>.Create(id)
        .ExecuteFunctionOnException(id.IsDefault, "The record Id is default.  Mo record retrieved.")
        .ExecuteAction((recordId) => _key = recordId)
        .ExecuteFunctionOnException(_entityProvider.RecordRequestAsync)
        .TaskSideEffectAsync(success: (record) => this.Item = record)
        .MapTaskToResultAsync();
```

### Result<TKey>.Create(id)

The first Step is to elevate the `TKey id` to a result.

```csharp
public static Result<T> Create(T? value) =>
    value is null
        ? new(new ResultException("T was null."))
        : new(value);
```

This converts the `TKey` value to a `Result<TKey>`.  In the process it does a null check and switches to the failure track if necessary.


### ExecuteFunctionOnException(id.IsDefault, "...")

Result is a `Result<TKey>`.

The next step is to check that `id` is a valid `Tkey`.  `TKey` implements `IEntityId` which provides `IsDefault`.    

```csharp
public Result<T> ExecuteFunctionOnException(bool test, string message)
{
    if (_exception is null && test)
        return Result<T>.Failure(message);

    return this;
}
```
`test` is only applied in the sucess state, and shifts to the fsilure state if `test` is true.

### ExecuteAction((recordId) => _key = recordId)

Result is a `Result<TKey>`.

The next step is to write the `id` to the parent object using a lambda expression.  This is an intended side effect.    

```csharp
public Result<T> ExecuteAction(Action<T>? success = null, Action<Exception>? failure = null)
{
    if (_exception is null && success != null)
        success(_value!);

    if (_exception is not null && failure != null)
        failure(_exception!);

    return this;
}
```

`success` or `failure` are executed based on state.

### ExecuteFunctionOnException(_entityProvider.RecordRequestAsync)

Result is a `Result<TKey>`.

The next step is to get the record.  The function is provided by the specific EntityProvider<TRecord>, and conforms to the `Func` signature.  It will either return a success `Result<TRecord>` if the data pipeline can find a record or a failure `Result<TRecord>` if it can't.

The important bit is the data pipeline is written in the functional programming paradigm and flows any errors up through the returned `Result<TRecord>`. 

```csharp
public async Task<Result<TOut>> ExecuteFunctionOnException<TOut>(Func<T, Task<Result<TOut>>> success)
{
    if (_exception is null)
        return await success(_value!);

    return Result<TOut>.Failure(_exception ?? _defaultException);
}
```

Note this is an `async` operation, so there's now the added complexity of the result wrapped in a `Task`.  

### TaskSideEffectAsync(success: (record) => this.Item = record)

Result is now wrapped in a Task: `Task<Result<TRecord>>`.
