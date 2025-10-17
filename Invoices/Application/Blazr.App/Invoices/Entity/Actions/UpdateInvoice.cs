/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

    public record UpdateInvoiceAction
    {
        private readonly DmoInvoice _invoice;

        public UpdateInvoiceAction(DmoInvoice invoice)
            => _invoice = invoice;

        public Result<InvoiceEntity> Dispatch(InvoiceEntity entity)
            => DroInvoice
                .CreateAsResult(_invoice, entity.InvoiceItems)
                .ExecuteTransform(record => InvoiceEntity.Load(record, entity));

        public static UpdateInvoiceAction Create(DmoInvoice invoice)
            => (new UpdateInvoiceAction(invoice));
    }

