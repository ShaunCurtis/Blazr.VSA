/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

/// <summary>
/// 
/// CustomerId is a Strongly Typed Id
/// It has three "States":
///   - New - where it has an Id that it has created.  This signals the data store that saving is an Add operation
///   - Existing - where the Id was inserted as part of the data pipeline read process.
///          This signals the data store that saving is an Update operation
///   - Default - where the id was newed up and has the default Guid Value
///          The data pipeline will need to create a new Id when it stores the record
///   
/// It implements custom:
/// -  ToString methods
/// -  Equality checking to compare only the Guids and ignore IsNew.
/// 
/// </summary>
/// <param name="Value"></param>
public readonly record struct CustomerId : IEntityId, IEquatable<CustomerId>
{
    public Guid Value { get; private init; }
    public bool IsNew { get; private init; }

    private CustomerId(Guid value)
        => Value = value;

    public CustomerId()
    {
        Value = Guid.CreateVersion7();
        IsNew = true;
    }

    public static CustomerId Load(Guid id)
        => id == Guid.Empty
            ? throw new InvalidGuidIdException()
            : new CustomerId(id);

    public static CustomerId NewId => new() { IsNew = true };

    public override string ToString()
        => Value.ToString();

    public string ToString(bool shortform)
        => Value.ToString().Substring(28);

    public bool Equals(CustomerId other)
        => this.Value == other.Value;

    public override int GetHashCode()
        => HashCode.Combine(this.Value);
}
