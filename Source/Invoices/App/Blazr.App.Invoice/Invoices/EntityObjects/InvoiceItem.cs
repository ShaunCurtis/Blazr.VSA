/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;

namespace Blazr.App.Invoice.Core;

internal sealed class InvoiceItem
{
    public CommandState State { get; set; }
        = CommandState.None;

    public DmoInvoiceItem Record { get; private set; }

    public bool IsDirty
        => this.State != CommandState.None;

    public DsrInvoiceItem AsRecord
        => new(this.Record, this.State);
    public InvoiceItem(DmoInvoiceItem item, bool isNew = false)
    {
        this.Record = item;

        if (isNew || item.Id.IsDefault)
            this.State = CommandState.Add;
    }

    public void Update(DmoInvoiceItem invoiceItem)
    {
        this.Record = invoiceItem;
        this.State = this.State.AsDirty;
    }

    public InvoiceItemId Id => this.Record.Id;
    public string Description => this.Record.Description;
    public decimal Amount => this.Record.Amount;
}
