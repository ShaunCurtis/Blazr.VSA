/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed record DmoInvoiceItem
{
    public InvoiceItemId Id { get; init; } = InvoiceItemId.Default;
    public InvoiceId InvoiceId { get; init; } = InvoiceId.Default;
    public Title Description { get; init; } = Title.Default;
    public Money Amount { get; init; }

    public static DmoInvoiceItem Create(InvoiceId invoiceId)
        => new() { Id = InvoiceItemId.Create, InvoiceId = invoiceId };
}
