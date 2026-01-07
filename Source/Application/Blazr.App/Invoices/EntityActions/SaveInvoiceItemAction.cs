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
            ? Update(entity, _invoiceItem)
            : Add(entity, _invoiceItem);

    public static Return<InvoiceEntity> Add(InvoiceEntity entity, DmoInvoiceItem invoiceItem)
        => ReturnT.Read(entity.InvoiceItems.Add(invoiceItem))
            .Map(items => entity.Mutate(items));

    public static Return<InvoiceEntity> Update(InvoiceEntity entity, DmoInvoiceItem invoiceItem)
        => entity.GetInvoiceItem(invoiceItem.Id)
            .Map(item => entity.InvoiceItems.Replace(item, invoiceItem))
            .Map(items => entity.Mutate(items));

    public static SaveInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new SaveInvoiceItemAction(invoiceItem));
}
