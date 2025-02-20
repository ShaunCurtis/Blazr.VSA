/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct InvoiceId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static InvoiceId Create => new(Guid.CreateVersion7());
    public static InvoiceId Default => new(Guid.Empty);

    public override string ToString()
    {
        return Value.ToString();
    }
}
