# Aggregates

Updating an object where the consequences of the change are limited to the object is simple.  Changing an object with business rules that encompass another object is complex.  The aggregate pattern addresses that problem.  

> An aggregate is a group of objects bound by one or more application rules.  The purpose of the aggregate is to ensure those rules are applied, and cannot be broken.  
 
An aggregate is a black box.  All changes are submitted to the black box, not the individual objects within it.  The black box applies the changes and runs the logic to ensure consistency.

> An aggregate only has purpose in a mutation context: you don't need aggregates to list or display data.  

An invoice is a good example of an aggregate. Delete a line item, and the aggregate needs to track the deletion of the item, calculate the new total amount and update the invoice.  Persist the aggregate to the data store, and the aggregate needs to hold the necessary state information to apply the appropriate update/add/delete actions as a *Unit of Work* to the data store.

## The Classic Aggregate

The classic version of the Invoice aggregate looks something like this:

```csharp
public class InvoiceAggregate
{
private List<InvoiceItems> items;

public <IEnumerable> Items => items.AsEnumerable;

public int InvoiceNo {public get; private set;}
//  More Invoice Properties

// Methods to update the items and invoice properties
}
```

This is a very *OOP* approach.  My Functional Programing approach is a little different.

## `EditState`

Any data object we create or mutate has state compared with it's origin.  A copy of a record from a data store is a good example.

It can be:

1. **New** - it does not exist in the data store and will need adding when *saved*.
1. **Clean** - it's the same as the data store.  It's unchanged, or been edited and then reverted to it's original state.
1. **Dirty** - it's been mutated from it's original state.
1. **Deleted** - it's marked for deletion, but will only be removed from the data store when the entity is saved.

the `EditState` object represents the state.

```csharp
public readonly record struct EditState
{
    public const int StateCleanIndex = 0;
    public const int StateNewIndex = 1;
    public const int StateDirtyIndex = 2;
    public const int StateDeletedIndex = -1;

    public int Index { get; private init; } = 0;
    public string Value { get; private init; } = "None";

    public EditState AsDirty => this.Index == StateCleanIndex ? EditState.Dirty : this; 

    public EditState() { }
    private EditState(int index, string value);

    public override string ToString();

    public static EditState Clean = new EditState(StateCleanIndex, "Clean");
    public static EditState New = new EditState(StateNewIndex, "New");
    public static EditState Dirty = new EditState(StateDirtyIndex, "Dirty");
    public static EditState Deleted = new EditState(StateDeletedIndex, "Deleted");

    public static EditState GetState(int index);
}
```

## `StateRecord<T>`

StateRecord is a immutable wrapper object for any `T`, that holds the `T` object instance an it's current edit state.

```csharp
public record StateRecord<T>
{
    public T Record { get; init; }
    public EditState State { get; init; }
    public Guid TransactionId { get; init; } = Guid.CreateVersion7();

    public StateRecord(T record, EditState state);
    public StateRecord(T record, EditState state, Guid transactionId);

    public bool IsDirty;
    public Result<StateRecord<T>> AsResult;

    public static StateRecord<T> Create(T record, EditState state, Guid? transactionId = null);
}
```

## `EntityState<T>`

EntityState is a mutable wrapper for `T` that tracks state.

```csharp
public sealed class EntityState<T>
    where T : notnull
{
    private StateRecord<T>? _lastState;
    private StateRecord<T> _currentState;
    private StateRecord<T> _baseState;

    public EditState State => _currentState.State;
    public T Record => _currentState.Record;

    public bool IsDirty => _currentState.State != EditState.Clean;

    public StateRecord<T> AsStateRecord => _currentState;

    public EntityState(T item, bool isNew = false);

    public Result Reset();
    public Result Update(T record, Guid transactionId);
    public Result MarkAsDeleted(Guid transactionId);
    public Result MarkAsPersisted();
    public Result RollBackLastUpdate(Guid transactionId);

    //.. privates
}
```

## Complex Entity

For the purposes of this article we will consider the WeatherForcast as a complex object.  It keeps things very simple and lets us focus on the process.

The base Entity class - the aggregate.

1. There are varius static constructors to create an instance of the class.
1. The `DmoWeatherForecast` is held internally in an `EntityState<DmoWeatherForecast>` object.
1. It's exposed by the entity in various immutable properties.  There's no way to change it once the class is loaded.

Hote: The entity root is an object in it's own right, it's not part of the *aggregate* class.


```csharp
public sealed partial class WeatherForecastEntity
{
    private readonly EntityState<DmoWeatherForecast> _weatherForecast;

    public event EventHandler<WeatherForecastId>? StateHasChanged;

    public WeatherForecastId Id => _weatherForecast.Record.Id;

    public DmoWeatherForecast WeatherForecast => _weatherForecast.Record;
    public StateRecord<DmoWeatherForecast> WeatherForecastRecord => _weatherForecast.AsStateRecord;
    public bool IsDirty => _weatherForecast.IsDirty;

    public WeatherForecastEntity(DmoWeatherForecast weatherForecast, bool? isNew = null)
    {
        _weatherForecast = new(weatherForecast, isNew ?? weatherForecast.Id.IsDefault);
    }

    public Result<WeatherForecastEntity> AsResult
        => Result<WeatherForecastEntity>.Create(this);

    public static WeatherForecastEntity Create()
            => new WeatherForecastEntity(new DmoWeatherForecast { Id = WeatherForecastId.Create }, true);

    public static WeatherForecastEntity Load(DmoWeatherForecast weatherForecast)
            => new WeatherForecastEntity(weatherForecast);

    public static WeatherForecastEntity Load(DmoWeatherForecast weatherForecast, bool isNew)
            => new WeatherForecastEntity(weatherForecast, isNew);
}
```

At this point we have our controlled environment: nothing can be changed from without.

## Domain Rules

The first step is to defines the domain rules for the entity.

This is done by adding the rules into the partial class.  Each rule is defined as a method with the signature `Func<Result>`: so used a *Monadic* function in Result chaining.

Here's the simple Weather Forecast example.  It doesn't allow the user to ceate a new forecast in the past.

```csharp
public sealed partial class WeatherForecastEntity
{
    private bool _processing;

    private Result ApplyRules(object? sender)
     => _processing
            ? Result.Failure("Rules already running.")
            : Result.Success()
        .ExecuteAction(() => _processing = true)
        .ExecuteFunction(NewDateNotInTheFutureRule)
        .ExecuteAction(() => this.StateHasChanged?.Invoke(sender ?? this, this.Id)
     );

    private Result NewDateNotInTheFutureRule()
        => (_weatherForecast.State == EditState.New && _weatherForecast.Record.Date.Value > DateOnly.FromDateTime(DateTime.Now))
            ? Result.Failure(new ValidationException("A new weather forecast must have a future date."))
            : Result.Success();
}
```

Note it's all private.  This is for consumption of internal methods only.  There' no use case for calling it externally.

## Entity Actions

All mutations are carried out by an `Action`.  The base definition is:

```csharp
public abstract record BaseAction<T>
    where T : BaseAction<T>
{
    public Guid TransactionId { get; protected init; } = default!;
    public object? Sender { get; protected init; } = default!;


    public T AddSender(object? sender)
        => (T)this with { Sender = sender };

    public T AddTransactionId(Guid transactionId)
        => (T)this with { TransactionId = transactionId };
}
```

Here's the `UpdateWeatherForecastAction` which mutates the weather forecast.

It:

1. Calls updte on the `EntityState`.

1. If that succeeds it applies the rules.
1. If those:
    1. Fail - it rolls back the change
    1. Succeed - raised the Entity changed event
    
1. Returns the entity object so the acton can be chained.
 
```csharp
public sealed partial class WeatherForecastEntity
{
    public record UpdateWeatherForecastAction : BaseAction<UpdateWeatherForecastAction>
    {
        public DmoWeatherForecast Item { get; private init; } = default!;

        private UpdateWeatherForecastAction() { }

        public Result<WeatherForecastEntity> ExecuteAction(WeatherForecastEntity entity)
            =>  entity._weatherForecast
                .Update(this.Item, this.TransactionId)
                .ExecuteFunction(() => entity.ApplyRules(this.Sender))
                .ExecuteAction(
                    hasNoException: () => entity.StateHasChanged?.Invoke(this.Sender, this.Item.Id),
                    hasException: ex => entity._weatherForecast.RollBackLastUpdate(this.TransactionId)
                )
                .ExecuteFunction<WeatherForecastEntity>(() => Result<WeatherForecastEntity>.Success(entity));

        public static UpdateWeatherForecastAction CreateAction(DmoWeatherForecast item)
            => new() { Item = item, TransactionId = Guid.NewGuid() };
    }
}
```