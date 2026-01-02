/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;


/// <summary>
/// CustomerId is a Strongly Typed Id
/// It has three "States":
///   - New - where it has an Id that it has created.  This signals the data store that saving is an Add operation
///   - Existing - where the Id was inserted as part of the data pipeline read process.
///          This signals the data store that saving is an Update operation
///   - Default - where the id was newed up and has the default Guid Value
///   
/// It implements custom ToString methods
/// 
/// It implements custom equality checking to compare only the Guids and ignore IsNew.  
/// </summary>
/// <param name="Value"></param>
public readonly record struct CustomerId(Guid Value) : IEntityId, IEquatable<CustomerId>
{
    public bool IsDefault => this == Default;
    public bool IsNew { get; private init; }

    public static CustomerId NewId() => new(Guid.CreateVersion7()) { IsNew = true};
    public static CustomerId Default => new(Guid.Empty);

    public override string ToString()
        => this.IsDefault ? "Default" : Value.ToString();

    public string ToString(bool shortform)
        => this.IsDefault ? "Default" : Value.ToString().Substring(28);

    public bool Equals(CustomerId other)
        => this.Value == other.Value;

    public override int GetHashCode()
        => HashCode.Combine(this.Value);
}
