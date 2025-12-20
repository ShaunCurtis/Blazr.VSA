/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core.Invoices;

namespace Blazr.App.Core;

public record AddInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public AddInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
        => entity.AddInvoiceItem(_invoiceItem);

    public static AddInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new AddInvoiceItemAction(invoiceItem));
}
