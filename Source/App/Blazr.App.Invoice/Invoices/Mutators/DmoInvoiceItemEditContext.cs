﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.EditStateTracker.Core;

namespace Blazr.App.Invoice.Core;

public sealed class DmoInvoiceItemEditContext
{
    private DmoInvoiceItem _baseRecord;
    public InvoiceItemId Id => _baseRecord.Id;

    [TrackState] public string Description { get; set; }
    [TrackState] public decimal Amount { get; set; }

    public DmoInvoiceItem AsRecord =>
        this.GetMutation(_baseRecord);

    public DmoInvoiceItemEditContext(DmoInvoiceItem record)
    {
        _baseRecord = record;
        this.Description = record.Description;
        this.Amount = record.Amount;
    }
    public bool IsDirty => _baseRecord != this.AsRecord;

    private DmoInvoiceItem GetMutation(DmoInvoiceItem item)
    {
        return item with
        {
            Description = this.Description,
            Amount = this.Amount,
        };
    }
}
