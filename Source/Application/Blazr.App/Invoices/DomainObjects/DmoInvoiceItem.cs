/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed record DmoInvoiceItem
{
    public InvoiceItemId Id { get; init; } = InvoiceItemId.NewId;
    public InvoiceId InvoiceId { get; init; } = InvoiceId.NewId;
    public Title Description { get; init; } = Title.Default;
    public Money Amount { get; init; }

    public static DmoInvoiceItem CreateNew(InvoiceId invoiceId)
        => new() { Id = InvoiceItemId.NewId, InvoiceId = invoiceId };
}
