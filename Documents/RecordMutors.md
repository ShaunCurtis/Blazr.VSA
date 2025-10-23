# Mutors

**Mutor** is a word I invented to describe a group of patterns for impementing the mutaton of immutable objects.

## The Record Mutor

Consider the `Customer` domain record.

```csharp
public sealed record DmoCustomer
{
    public CustomerId Id { get; init; }
    public Title Name { get; init; }
}
```

In order to edit this record in the UI, we need access to read/write fields representing the editable fields: in this case `Name`.

This is where we use the *Mutor* pattern.  Here's the *Mutor* for the Customer UI editor:

```csharp
public sealed class CustomerRecordMutor
{
    public DmoCustomer BaseRecord { get; private init; }
    public bool IsDirty => !this.AsRecord.Equals(BaseRecord);
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

    public DmoCustomer AsRecord => this.BaseRecord with
    {
        Name = new(this.Name ?? "No Name Set")
    };

    public void Reset()
        => this.SetFields();

    public static CustomerRecordMutor Create(DmoCustomer record)
        => new CustomerRecordMutor(record);
}
```

It's locked down.

1. A `CustomerRecordMutor`can only be created through the static `Create` method.
1. The only editable field is `Name`.
1. The only method that changes the internal state is `Reset`.
1. The current state is available through `IsDirty`.
1. The current *record* (to save) is available through `AsRecord`.

How do you use it?

In Blazor the `CustomerRecordMutor` instance is the `model` for the `EditContext`.  On `Save`, submit the record from `AsRecord` to the data pipeline.