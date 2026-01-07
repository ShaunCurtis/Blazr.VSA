/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct InvoiceItemId : IEntityId, IEquatable<InvoiceItemId>
{
    public Guid Value { get; private init; }
    public bool IsNew { get; private init; }

    public InvoiceItemId()
    {
        Value = Guid.CreateVersion7();
        IsNew = true;
    }

    private InvoiceItemId(Guid value)
        => Value = value;

    public static InvoiceItemId Load(Guid id)
        => id == Guid.Empty
        ? throw new InvalidGuidIdException()
        : new InvoiceItemId(id);

    public static InvoiceItemId NewId => new() { IsNew = true };

    public override string ToString()
        => Value.ToString();

    public string ToString(bool shortform)
        => Value.ToString().Substring(28);

    public bool Equals(InvoiceItemId other)
        => this.Value == other.Value;

    public override int GetHashCode()
        => HashCode.Combine(this.Value);
}

