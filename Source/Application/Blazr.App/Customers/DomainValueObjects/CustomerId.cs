/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct CustomerId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public bool IsNew { get; private init; }

    public static CustomerId NewId() => new(Guid.CreateVersion7()) { IsNew = true};
    public static CustomerId Default => new(Guid.Empty);

    public override string ToString()
        => this.IsDefault ? "Default" : Value.ToString();

    public string ToString(bool shortform)
        => this.IsDefault ? "Default" : Value.ToString().Substring(28);
}
