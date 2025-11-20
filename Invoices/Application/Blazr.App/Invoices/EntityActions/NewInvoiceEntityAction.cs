/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record NewInvoiceEntityAction
{
    private readonly DmoInvoice _invoice;

    public NewInvoiceEntityAction(DmoInvoice invoice)
        => _invoice = invoice;

    public Bool<InvoiceEntity> Dispatcher()
        => InvoiceEntity.CreateNewEntity(_invoice).ToBoolT();

    public static UpdateInvoiceAction Create(DmoInvoice invoice)
            => (new UpdateInvoiceAction(invoice));
}
