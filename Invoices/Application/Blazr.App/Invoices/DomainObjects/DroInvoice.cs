/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App;

public record DroInvoice : CollectionRecord<DmoInvoice, DmoInvoiceItem>
{
    public DroInvoice(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        : base(invoice, invoiceItems) { }

    public Result<DroInvoice> ToResult => Result<DroInvoice>.Create(this);

    public static DroInvoice Create(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => new DroInvoice(invoice, invoiceItems);

    public static Result<DroInvoice> CreateAsResult(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => Result<DroInvoice>.Create(Create(invoice,invoiceItems));
}
