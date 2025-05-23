# Commands

A command is defined as:

```csharp
Result CommandAsync(CommandRequest command);
```

Commands have one of three actions: 

1. Update - overwrite the record in the data store.
2. Delete - delete the record from the data store.
3. Add - Add the record to the data store.
 
Command functionality is defined in a `CommandState` readonly struct.  using an `enum` is problematic when you cross domain boundaries and API interfaces.  Search "c# why you shouldn't use emums" for more information on the topic.

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

    public override string ToString()
    {
        return this.Value;
    }

    public CommandState AsDirty
        => this.Index == 0 ? CommandState.Update : this;

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

## The CQS Request

We define a generic command request object:

```
public readonly record struct CommandRequest<TRecord>(TRecord Item, CommandState State );
```

## The Mediator Request

The Mediator request is record specific: here's the Customer Record Request.

```csharp
public readonly record struct CustomerCommandRequest(DmoCustomer Item, CommandState State) : IRequest<Result<CustomerId>>;
```

## The Mediator Handler

The handler is where the work is done.  The handler is a `IRequestHandler` that takes in the request and returns a `Result<CustomerId>`.  The handler is responsible for getting the data from the database and returning it in the requested format.  In this case, it uses the registered generic `ICommandBroker` to get the data.

```csharp
public sealed record CustomerCommandHandler : IRequestHandler<CustomerCommandRequest, Result<CustomerId>>
{
    private ICommandBroker _broker;
    private IMessageBus _messageBus;

    public CustomerCommandHandler(ICommandBroker broker, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _broker = broker;
    }

    public async Task<Result<CustomerId>> Handle(CustomerCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _broker.ExecuteAsync<DboCustomer>(new CommandRequest<DboCustomer>(
            Item: DboCustomerMap.Map(request.Item),
            State: request.State
        ), cancellationToken);

        if (!result.HasSucceeded(out DboCustomer? record))
            return result.ConvertFail<CustomerId>();

        _messageBus.Publish<DmoCustomer>(DboCustomerMap.Map(record));

        return Result<CustomerId>.Success(new CustomerId(record.CustomerID));
    }
}
```

### The CQS Broker

The CQS broker implements the `ICommandBroker`.  The broker is responsible for executing the command on the data store.  The broker is a generic class that takes a `DbContext` type as a parameter.

Note: `TRecord` needs to implement `ICommandEntity`.  This is a marker interface that indicates the record can be updated, added or deleted.

```csharp
public sealed class CommandServerBroker<TDbContext>
    : ICommandBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public CommandServerBroker(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<Result<TRecord>> ExecuteAsync<TRecord>(CommandRequest<TRecord> request, CancellationToken cancellationToken = new())
        where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();

        if ((request.Item is not ICommandEntity))
            return Result<TRecord>.Fail(new CommandException($"{request.Item.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly."));

        var stateRecord = request.Item;

        // First check if it's new.
        if (request.State == CommandState.Add)
        {
            dbContext.Add<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

            return result == 1
                ? Result<TRecord>.Success(request.Item)
                : Result<TRecord>.Fail( new CommandException("Error adding Record"));
        }

        // Check if we should delete it
        if (request.State == CommandState.Delete)
        {
            dbContext.Remove<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
            
            return result == 1
                ? Result<TRecord>.Success(request.Item)
                : Result<TRecord>.Fail(new CommandException( "Error deleting Record"));
        }

        // Finally it changed
        if (request.State == CommandState.Update)
        {
            dbContext.Update<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

            return result == 1
                ? Result<TRecord>.Success(request.Item)
                : Result<TRecord>.Fail(new CommandException("Error saving Record"));
        }

        return Result<TRecord>.Fail(new CommandException("Nothing executed.  Unrecognised State."));
    }
}
```

## The Result

All commands only return status information.  It's bad practice to  return a `null` without explaining why!

The CQS command returns a `Result<TRecord>`.  Where the data store generates the Id, it's imperitive to return the new Id.  In Entity Framework, *Add* updates the Id in the provided record.  As the handler is generic, we don't know the Id type, so we return the full record.  The record specific caller can extract the correct typed Id.  In our case, where the Id is generated by the client, so the record is returned as submitted.

The Mediator record specific handler converts the `Result<TRecord>`. Thw `CustomerCommandHandler` returns a `Result<CustomerId>`.
