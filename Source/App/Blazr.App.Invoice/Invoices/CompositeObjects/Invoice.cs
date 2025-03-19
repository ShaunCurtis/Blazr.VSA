/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;

namespace Blazr.App.Invoice.Core;

internal sealed class Invoice
{
    public CommandState State { get; set; }
        = CommandState.None;

    public DmoInvoice Record { get; private set; }

    public bool IsDirty
        => this.State != CommandState.None;

    public InvoiceRecord AsRecord(List<InvoiceItemRecord> items)
        => new(this.Record, items, this.State);

    public Invoice(DmoInvoice item, bool isNew = false)
    {
        this.Record = item;

        if (isNew || item.Id.IsDefault)
            this.State = CommandState.Add;
    }

    public InvoiceId Id => this.Record.Id;

    public void Update(DmoInvoice invoice)
    {
        this.Record = invoice;
        this.State = this.State.AsDirty;
    }
}
