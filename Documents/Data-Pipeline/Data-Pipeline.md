#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

- Entity Framework is used as the **Object Request Mapper**, implementing the basic repository and unit of work patterns.  

- **Command/Query Separation** to provide a thin layer over EF to separate commands from queries.  

- Mediatr to totally decouple the UI and Presentation from the Core Domain and Application logic code.


All data is *READONLY*: either `record` or `readonly record struct`.  Data retrieved from a data source is a **copy** of the data within the data source.  Data is mutated by creating a new copy, not by changing the copy.

The data pipeline performs three activities:

1. Querying for single item - called a *RecordRequest*.
2. Querying for a list of items - called a *ListRequest*.
3. Submitting a command to update the data store - called a *Command*. - the *Command State* defines the action - *add*, *update* or *delete*.

## The Data Object Tree

First the object to transfer data into from the database through EF.

```csharp
public sealed record DboCustomer : ICommandEntity
{
    [Key] public Guid CustomerID { get; init; }
    public string CustomerName { get; init; } = string.Empty;
}
```

Next the domain entity.  Note the switch to value objects - in this case a strongly typed Id.

```csharp
public sealed record DmoCustomer
{
    public CustomerId Id { get; init; } = CustomerId.Default;
    public string CustomerName { get; init; } = string.Empty;
}
```
With a Mapper:

```csharp
public sealed class DboCustomerMap : IDboEntityMap<DboCustomer, DmoCustomer>
{
    public DmoCustomer MapTo(DboCustomer item)
        => Map(item);

    public DboCustomer MapTo(DmoCustomer item)
        => Map(item);

    public static DmoCustomer Map(DboCustomer item)
        => new()
        {
            Id = new(item.CustomerID),
            CustomerName = new(item.CustomerName),
        };

    public static DboCustomer Map(DmoCustomer item)
        => new()
        {
            CustomerID = item.Id.Value,
            CustomerName = item.CustomerName
        };
}
```


```csharp
```