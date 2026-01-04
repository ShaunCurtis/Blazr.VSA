/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core.Invoices;

namespace Blazr.App.Core;

public record DeleteInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public DeleteInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
            => entity.GetInvoiceItem(_invoiceItem.Id)
                .Bind(item => entity.Mutate(entity.InvoiceItems.Remove(item)));

    public static DeleteInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => new DeleteInvoiceItemAction(invoiceItem);
}
