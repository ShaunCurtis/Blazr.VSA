/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core.Invoices;
using System.Collections.Immutable;

namespace Blazr.App.Core;

public record DeleteInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public DeleteInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
            => entity
                // Get the invoice Item
                .GetInvoiceItem(_invoiceItem.Id)
                // Create a new list with the item removed
                .Map(item =>  entity.InvoiceItems.Remove(item))
                // Create and return a new Entity with the new list
                .Map(items => entity.Mutate(items));

    public static DeleteInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => new DeleteInvoiceItemAction(invoiceItem);
}
