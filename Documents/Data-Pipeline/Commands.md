# Commands

A command can be defined like this:

```csharp
CommandResult CommandAsync(CommandRequest command);
```

Commands have one of three actions: 

1. Update - overwrite the record in the data store.
2. Delete - delete the record from the data store.
3. Add - Add the record to the data store.
 
These can be defined in a `CommandState` struct.  An enum is not used because the action state can cross domain boundaries and API interfaces.  Search "c# why you shouldn't use emums" for more information on the topic.

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

we can define a generic command request object:

```
public readonly record struct CommandRequest<TRecord>(TRecord Item, CommandState State );
```

## The Mediator Request

Here's the Customer Record Request.  We pass in the new copy of the record and the CommandState.

```csharp
public readonly record struct CustomerCommandRequest(DmoCustomer Item, CommandState State) : IRequest<Result<CustomerId>>;
```

## The Mediator Handler

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

Commands return a standard `Result`.

```csharp
public readonly record struct Result
{
    private Exception? _error { get; init; }

    public bool IsSuccess { get; init; } = true;
    public bool IsFailure => !IsSuccess;
    
    private Result(Exception error)
    {
        IsSuccess = false;
        _error = error;
    }

    public bool HasFailed([NotNullWhen(true)] out Exception? exception)
    {
        if (this.IsFailure)
            exception = _error;
        else
            exception = default;

        return this.IsFailure;
    }
```

## The Handler

The Core domain defines a *contract* interface that it uses to get items.  It doesn't care where they come from.  You may be implementing Blazor Server and calling directly into the database, or Blazor WASM and making API calls.

This is `ICommandHandler`.  There are two implementations: a generic handler and a specific object based handler.  They both define a single `ExecuteAsync` method.

```csharp
public interface ICommandHandler
{
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class;
}

public interface ICommandHandler<TRecord>
        where TRecord : class
{
    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<TRecord> request);
}

public interface ICommandHandler<TRecord, TDbo>
        where TRecord : class
        where TDbo : class
{
    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<TRecord> request);
}
```

### Server Handler

The handler basic structure looks like this.  `TDbContext` defines the `DbContext` to obtain through the DbContext Factory service.   

```csharp
public sealed class CommandServerHandler<TDbContext>
    : ICommandHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public CommandServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        // Try and get a registered custom handler
        var _customHandler = _serviceProvider.GetService<ICommandHandler<TRecord>>();

        // If one exists execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If not run the base handler
        return await this.ExecuteCommandAsync<TRecord>(request);
    }

    private async ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request)
    where TRecord : class
    {
        //...
    }
}
```

`ExecuteAsync` checks for a specific record registered handler.  If one exists it uses that, if not it calls the internal `ExecuteCommandAsync`.

The default server method looks like this.  It gets a *unit of work* `DbContext` from the factory, calls the relevant Update/Add/Delete method on the context and returns a `CommandResult` based on the result.

```
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
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Added")
                : CommandResult.Failure("Error adding Record");
        }

        // Check if we should delete it
        if (request.State == CommandState.Delete)
        {
            dbContext.Remove<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Deleted")
                : CommandResult.Failure("Error deleting Record");
        }

        // Finally it changed
        if (request.State == CommandState.Update)
        {
            dbContext.Update<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Updated")
                : CommandResult.Failure("Error saving Record");
        }

        return CommandResult.Failure("Nothing executed.  Unrecognised State.");
    }
```
### API Handler

```csharp
public sealed class CommandAPIHandler
    : ICommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public CommandAPIHandler(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Uses a specific handler if one is configured in DI
    /// If not, uses a generic handler using the APIInfo attributes to configure the HttpClient request  
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        ICommandHandler<TRecord>? _customHandler = null;

        _customHandler = _serviceProvider.GetService<ICommandHandler<TRecord>>();

        // Get the custom handler
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        return await CommandAsync<TRecord>(request);
    }


    public async ValueTask<CommandResult> CommandAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        var attribute = Attribute.GetCustomAttribute(typeof(TRecord), typeof(APIInfo));

        if (attribute is null || !(attribute is APIInfo apiInfo))
            throw new DataPipelineException($"No API attribute defined for Record {typeof(TRecord).Name}");

        using var http = _httpClientFactory.CreateClient(apiInfo.ClientName);

        var apiRequest = CommandAPIRequest<TRecord>.FromRequest(request);

        var httpResult = await http.PostAsJsonAsync<CommandAPIRequest<TRecord>>($"/API/{apiInfo.PathName}/Command", apiRequest, request.Cancellation);

        if (!httpResult.IsSuccessStatusCode)
            return CommandResult.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var commandAPIResult = await httpResult.Content.ReadFromJsonAsync<CommandAPIResult<Guid>>();

        CommandResult? commandResult = null;

        if (commandAPIResult is not null)
            commandResult = commandAPIResult.ToCommandResult();

        return commandResult ?? CommandResult.Failure($"No data was returned");
    }
}
```


