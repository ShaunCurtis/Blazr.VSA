/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core.Invoices;

namespace Blazr.App.Core;

public record UpdateInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public UpdateInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
        => entity.GetInvoiceItem(_invoiceItem)
            .Bind(item => entity.Mutate(entity.InvoiceItems.Replace(item, _invoiceItem)));

    public static UpdateInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new UpdateInvoiceItemAction(invoiceItem));
}
