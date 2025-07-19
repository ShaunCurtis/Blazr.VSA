# Data Pipeline Commands

Commands mutate the data store.  They are the only way to change the data store.  Commands are always executed in a transaction.  If the command fails, the transaction is rolled back.  If the command succeeds, the transaction is committed.

A command request if defined by:

```csharp
public readonly record struct CommandRequest<TRecord>(
    TRecord Item, 
    CommandState State, 
    CancellationToken Cancellation
    );
```

Where the `CommandState` is:

```csharp
public readonly record struct CommandState
{
    public int Index { get; private init; } = 0;
    public string Value { get; private init; } = "None";

    public CommandState() { }

    private CommandState(int index, string value)
    {
        Index = index;
        Value = value;
    }

    public static CommandState None = new CommandState(0, "None");
    public static CommandState Add = new CommandState(1, "Add");
    public static CommandState Update = new CommandState(2, "Update");
    public static CommandState Delete = new CommandState(-1, "Delete");

    public static CommandState GetState(int index)
        => (index) switch
        {
            1 => CommandState.Add,
            2 => CommandState.Update,
            -1 => CommandState.Delete,
            _ => CommandState.None,
        };
}
```

The `CommandRequest` is passed to the `CommandHandler`.  The `ICommandHandler` is defined as:

```csharp
public generic interface ICommandHandler
{
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class;
}
```

And the generic server side implementation:

```csharp
public sealed class CommandServerHandler<TDbContext>
    : ICommandHandler
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public CommandServerHandler(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        return await this.ExecuteCommandAsync<TRecord>(request);
    }

    private async ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request)
    where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();

        if ((request.Item is not ICommandEntity))
            return CommandResult.Failure($"{request.Item.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly.");

        var stateRecord = request.Item;

        // First check if it's new.
        if (request.State == CommandState.Add)
        {
            dbContext.Add<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None);

            return result == 1
                ? CommandResult.SuccessWithKey(request.Item)
                : CommandResult.Failure("Error adding Record");
        }

        // Check if we should delete it
        if (request.State == CommandState.Delete)
        {
            dbContext.Remove<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None);
            
            return result == 1
                ? CommandResult.Success()
                : CommandResult.Failure("Error deleting Record");
        }

        // Finally it changed
        if (request.State == CommandState.Update)
        {
            dbContext.Update<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None);

            return result == 1
                ? CommandResult.Success()
                : CommandResult.Failure("Error saving Record");
        }

        return CommandResult.Failure("Nothing executed.  Unrecognised State.");
    }
}
```

Points:

1. We're using an `IDbContextFactory` and *unit of work* `DbContexts` scoped to the execution method.

1. The `CommandState` dictates the actual action. 
 
The Handler returns a `CommandResult`.  There are two incarnations:

```csharp
public readonly record struct CommandResult : IDataResult
{
    public bool Successful { get; private init; }
    public object? KeyValue { get; private init; }
    public Exception? Exception { get; private init; } = new Exception("This Result instance has not been Initialized.");

    public string? Message => this.Exception?.Message ?? null;

    //Various methods.
}
```

```csharp
public readonly record struct CommandResult<TKeyValue> : IDataResult
{
    public bool Successful { get; private init; } = false;
    public TKeyValue? KeyValue { get; private init; }
    public Exception? Exception { get; private init; } = new Exception("This Result instance has not been Initialized.");

    public string? Message => this.Exception?.Message ?? null;
```
