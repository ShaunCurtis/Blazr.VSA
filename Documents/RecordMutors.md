# Mutors

**Mutor** is a word I invented to describe a group of patterns for implementing the mutaton of immutable objects.

## The Record Mutor

Consider the `Customer` domain record.

```csharp
public sealed record DmoCustomer
{
    public CustomerId Id { get; init; }
    public Title Name { get; init; }
}
```

To edit this record in the UI, we need access to read/write fields representing the editable data: in this case just `Name`.

This is where we use the *Mutor* pattern.  Here's the *Mutor* for the Customer UI editor:

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

And the base abstract class `RecordMutor`

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

It's locked down.

1. A `CustomerRecordMutor`can only be initialised through the static `Load` and `Create` methods.
1. The only editable field is `Name`.
1. The only method that changes the internal state is `Reset`.
1. The current state is available through `IsDirty`.
1. The *edited record* (to save) is available through `Record`.

How do you use it?

In Blazor the `CustomerRecordMutor` instance is the `model` for the `EditContext`.  On `Save`, submit the record from `AsRecord` to the data pipeline.

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