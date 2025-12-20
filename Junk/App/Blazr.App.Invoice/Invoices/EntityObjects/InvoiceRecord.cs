/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;

namespace Blazr.App.Invoice.Core;

public record InvoiceRecord(DmoInvoice Record, IEnumerable<InvoiceItemRecord> Items, CommandState State)
{
    public bool IsDirty
        => this.State != CommandState.None;
}
