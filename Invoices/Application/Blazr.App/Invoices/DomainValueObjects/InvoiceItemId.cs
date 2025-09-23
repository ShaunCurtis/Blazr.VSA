/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct InvoiceItemId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static InvoiceItemId Create => new(Guid.CreateVersion7());
    public static InvoiceItemId Default => new(Guid.Empty);

    public InvoiceItemId ValidatedId => this.IsDefault ? Create : this;

    public override string ToString()
    {
        return this.IsDefault ? "Default" : Value.ToString();
    }
}
