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

    public Result<InvoiceEntity> Dispatch(InvoiceEntity entity)
        => entity.ToResult
            .SwitchToExceptionOnTrue(
                test: entity.Items.Any(item => item.Id == _invoiceItem.Id),
                message: "The record aready exists in the Invoice Items"
            )
           .ExecuteTransaction(_entity =>
               {
                   var invoiceItems = entity.Items.ToList();
                   invoiceItems.Add(_invoiceItem);
                   return _entity.Mutate(invoiceItems);
               }
            )
            .ExecuteTransaction(InvoiceEntity.ApplyEntityRules);

    public static AddInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new AddInvoiceItemAction(invoiceItem));
}
