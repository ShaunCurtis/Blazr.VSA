# Record Mutors

> **Mutor** is my word to describe a set of patterns for implementing the mutaton of immutable objects.

Record Mutors are classes that provide a read/write *view* over an immutable record.  Thw mutor maintains the editable fields as mutable properties, and provides methods to load from and save to the immutable record.  It maintains a copy of the original record and thus provides state information such as `IsDirty` and `IsNew`.  A copy of the current record is obtained from the `Record` property.  Mutors can only be created though two *static* methods: `Load` and `Create`.  Mutors are typically used as the `model` for an edit form.

The mutors in the solution use the `[TrackState]` custom attribute to tell the `EditStateTracker` in the Blazor `EditForm` to track the individual property state..

## The Record Mutor

Consider the `Customer` domain record.

```csharp
public sealed record DmoCustomer
{
    public CustomerId Id { get; init; }
    public Title Name { get; init; }
}
```

The UI needs access to read/write fields representing the editable data in the record: in this case just `Name`.

This is where we use the *Mutor* pattern.  


First an abstract base class:

```csharp
public abstract class RecordMutor<TRecord>
    where TRecord : class
{
    public TRecord BaseRecord { get; protected set; } = default!;
    public bool IsDirty => !this.Record.Equals(BaseRecord);
    public bool IsNew { get; protected set; }
    public virtual TRecord Record { get; } = default!;

    public EditState State => (this.IsNew, this.IsDirty) switch
    {
        (true, _) => EditState.New,
        (false, false) => EditState.Clean,
        (false, true) => EditState.Dirty,
    };
}
```

And the Customer record UI mutor:

```csharp
public sealed class CustomerRecordMutor : RecordMutor<DmoCustomer>, IRecordMutor<DmoCustomer>
{
    [TrackState] public string? Name { get; set; }

    private CustomerRecordMutor(DmoCustomer record)
    {
        this.BaseRecord = record;
        this.SetFields();
    }

    private void SetFields()
    {
        this.Name = this.BaseRecord.Name.Value;
    }

    public override DmoCustomer Record => this.BaseRecord with
    {
        Name = new(this.Name ?? "No Name Set")
    };

    public void Reset()
        => this.SetFields();

    public static CustomerRecordMutor Load(DmoCustomer record)
        => new CustomerRecordMutor(record);

    public static CustomerRecordMutor Create()
        => new CustomerRecordMutor(DmoCustomer.CreateNew()) { IsNew = true };
}
```

It's locked down.

1. A `CustomerRecordMutor`can only be initialised through the static `Load` and `Create` methods.
1. The only editable field is `Name`.
1. The only method that changes the internal state is `Reset`.
1. The current state is available through `IsDirty`.
1. The *mutated record* (to save) is available through `Record`.

How do you use it?

In Blazor the `CustomerRecordMutor` instance is the `model` for the `EditContext`.  On `Save`, submit the record from the `Record` property into the data pipeline.

## Emulating the Process

We can emulate the UI process in a simple test.  See the inline commentary for details.

```csharp
    [Fact]
    public async Task UpdateACustomer()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the test item and it's Id from the Test Provider
        var controlItem = _testDataProvider.Customers.Skip(Random.Shared.Next(2)).First();
        var controlRecord = this.AsDmoCustomer(controlItem);
        var controlId = controlRecord.Id;

        // Get the record from the data pipeline
        var customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(controlId));
        Assert.True(customerResult.HasValue);

        // Load the mutor
        var mutor = CustomerRecordMutor.Load(customerResult.Value!);

        // emulate a UI Edit
        mutor.Name = $"{mutor.Name} - Update";

        //emulate the Validation process 
        var validator = new CustomerRecordMutorValidator();
        var validateResult = validator.Validate(mutor);
        var editedRecord = mutor.Record;
        Assert.True(validateResult.IsValid);

        // Emulate the save button
        var customerUpdateResult = await mediator.DispatchAsync(CustomerCommandRequest.Create(mutor.Record, mutor.State));

        // Get the new record from the data pipeine
        customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(controlId));

        // check it matches the test record
        Assert.False(customerResult.HasException);
        Assert.Equivalent(editedRecord, customerResult.Value);
    }
```