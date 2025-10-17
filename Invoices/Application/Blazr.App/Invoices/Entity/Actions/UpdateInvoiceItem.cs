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

    public Result<InvoiceEntity> Dispatch(InvoiceEntity entity)
    {
        return Result<DmoInvoiceItem>
            .Create(
                value: entity.InvoiceItems.SingleOrDefault(item => item.Id == _invoiceItem.Id),
                errorMessage: "No record exists in the Invoice Items.")
            .ExecuteFunction<List<DmoInvoiceItem>>(item =>
            {
                var invoiceItems = entity.InvoiceItems.ToList();
                invoiceItems.Remove(item);
                invoiceItems.Add(_invoiceItem);
                return invoiceItems;
            })
            .ExecuteTransform(invoiceItems => DroInvoice.CreateAsResult(entity.Invoice, invoiceItems))
            .ExecuteTransform(dro => InvoiceEntity.Load(dro, entity));
    }

    public static UpdateInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new UpdateInvoiceItemAction(invoiceItem));
}
