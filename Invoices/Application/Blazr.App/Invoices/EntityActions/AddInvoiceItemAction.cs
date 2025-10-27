/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record AddInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public AddInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Result<InvoiceMutor> Dispatch(InvoiceMutor mutor)
    {
        if (mutor.ContainsInvoiceItem(_invoiceItem))
            return Result<InvoiceMutor>.Failure("The record aready exists in the Invoice Items");
 
        var newInvoiceItems = mutor.CurrentEntity.InvoiceItems
            .ToList()
            .AddItem(_invoiceItem);
        
        return mutor.Mutate(newInvoiceItems);
    }

    public static AddInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new AddInvoiceItemAction(invoiceItem));
}
