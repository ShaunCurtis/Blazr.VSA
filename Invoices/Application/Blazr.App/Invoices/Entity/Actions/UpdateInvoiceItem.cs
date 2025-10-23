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
        => entity.ToResult
        .ExecuteTransaction(_entity =>
            {
                var invoiceItem = entity.Items.SingleOrDefault(item => item.Id == _invoiceItem.Id);
                if (invoiceItem == null)
                    return Result<InvoiceEntity>.Failure("No record exists in the Invoice Items.");

                var invoiceItems = entity.Items.ToList();
                invoiceItems.Remove(invoiceItem);
                invoiceItems.Add(_invoiceItem);
                return entity.Mutate(invoiceItems);
            }
        )
        .ExecuteTransaction(InvoiceEntity.ApplyEntityRules);

    public static UpdateInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new UpdateInvoiceItemAction(invoiceItem));
}
