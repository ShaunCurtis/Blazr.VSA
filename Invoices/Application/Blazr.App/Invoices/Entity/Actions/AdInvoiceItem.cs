using Blazr.Diode;

using static Blazr.App.Core.AppDictionary;

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
    {
        return Result.Success()
            .SwitchToException(
                test: entity.InvoiceItems.Any(item => item.Id == _invoiceItem.Id),
                message: "The record aready exists in the Invoice Items"
            )
            .ExecuteFunction<List<DmoInvoiceItem>>(() =>
            {
                var invoiceItems = entity.InvoiceItems.ToList();
                invoiceItems.Add(_invoiceItem);
                return invoiceItems;
            })
            .ExecuteTransform(invoiceItems => DroInvoice.CreateAsResult(entity.Invoice, invoiceItems))
            .ExecuteTransform(dro => InvoiceEntity.Load(dro, entity));
    }

    public static AddInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new AddInvoiceItemAction(invoiceItem));
}
