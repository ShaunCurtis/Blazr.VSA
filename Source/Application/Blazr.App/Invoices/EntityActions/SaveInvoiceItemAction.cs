/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core.Invoices;

namespace Blazr.App.Core;

public record SaveInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public SaveInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
            => entity.InvoiceItems.Any(_item => _invoiceItem.Id.Equals(_item.Id))
                  ? entity.Mutate(entity.InvoiceItems.Replace(entity.GetInvoiceItem(_invoiceItem.Id).Value!, _invoiceItem))
                  : entity.Mutate(entity.InvoiceItems.Add(_invoiceItem));

    public static SaveInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new SaveInvoiceItemAction(invoiceItem));
}
