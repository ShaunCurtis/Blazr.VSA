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

    public Result<InvoiceMutor> Dispatch(InvoiceMutor mutor)
    {
        var invoiceItemResult = mutor.GetInvoiceItem(_invoiceItem);
        if (invoiceItemResult.HasException)
            return Result<InvoiceMutor>.Failure("No record exists in the Invoice Items.");

        var newInvoiceItems = mutor.CurrentEntity.InvoiceItems.ToList()
            .RemoveItem(invoiceItemResult.Value!)
            .AddItem(_invoiceItem);

        return mutor.Mutate(newInvoiceItems);
    }

    public static UpdateInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new UpdateInvoiceItemAction(invoiceItem));
}
