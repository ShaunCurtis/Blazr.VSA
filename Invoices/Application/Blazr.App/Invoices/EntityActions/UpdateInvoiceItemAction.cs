/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record UpdateInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public UpdateInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Return<InvoiceMutor> Dispatch(InvoiceMutor mutor)
        => mutor
            .GetInvoiceItem(_invoiceItem)
            .Map(invoiceItem => mutor.CurrentEntity.InvoiceItems
                .ToList()
                .RemoveItem(invoiceItem)
                .AddItem(_invoiceItem))
            .Bind(mutor.Mutate);

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
        => entity
                .GetInvoiceItem(_invoiceItem)
                .Map(invoiceItem => entity.InvoiceItems
                    .ToList()
                    .RemoveItem(invoiceItem)
                    .AddItem(_invoiceItem))
                .Bind(entity.MutateWithEntityRulesApplied);

    public static UpdateInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new UpdateInvoiceItemAction(invoiceItem));
}
