/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core.Invoices;

namespace Blazr.App.Core;

public record UpdateInvoiceAction
{
    private readonly DmoInvoice _invoice;

    public UpdateInvoiceAction(DmoInvoice invoice)
        => _invoice = invoice;

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
        // return a new Entity with the provided invoice
        => entity.Mutate(_invoice).ToReturnT;

    public static UpdateInvoiceAction Create(DmoInvoice invoice)
            => (new UpdateInvoiceAction(invoice));
}

