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
    {
        var invoiceItemResult = mutor.GetInvoiceItem(_invoiceItem);

        if (invoiceItemResult.HasException)
            return Result<InvoiceMutor>.Failure(invoiceItemResult.Exception!);

        var newInvoiceItems = mutor.CurrentEntity.InvoiceItems
            .ToList()
            .RemoveItem(invoiceItemResult.Value!);

        return mutor.Mutate(newInvoiceItems);
    }

    public static DeleteInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => new DeleteInvoiceItemAction(invoiceItem);
}
