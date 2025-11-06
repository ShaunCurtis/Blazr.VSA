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

    public Result<InvoiceEntity> Dispatcher(InvoiceEntity entity)
        => entity
        .CheckInvoiceItemDoesNotExist(_invoiceItem)
            .ExecuteFunction(invoiceItem => entity.InvoiceItems
                    .ToList()
                    .AddItem(invoiceItem)
            )
            .ExecuteTransform(entity.CreateWithRulesValidation);

    public static AddInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new AddInvoiceItemAction(invoiceItem));
}
