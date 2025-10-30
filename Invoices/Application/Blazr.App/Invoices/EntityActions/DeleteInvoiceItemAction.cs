/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record DeleteInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public DeleteInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Result<InvoiceMutor> Dispatch(InvoiceMutor mutor)
        => mutor.GetInvoiceItem(_invoiceItem)
            .ExecuteFunction(invoiceItem => mutor.CurrentEntity.InvoiceItems
                .ToList()
                .RemoveItem(invoiceItem))
            .ExecuteTransform(mutor.Mutate);

    public static DeleteInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => new DeleteInvoiceItemAction(invoiceItem);
}
