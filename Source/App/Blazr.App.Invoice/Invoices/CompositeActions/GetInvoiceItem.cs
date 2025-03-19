/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Invoice.Core.InvoiceActions;

namespace Blazr.App.Invoice.Core;
public static partial class InvoiceActions
{
    public readonly record struct GetInvoiceItemAction(InvoiceItemId id);
}

public sealed partial class InvoiceComposite
{
    /// <summary>
    /// Gets an existing Invoice Item.  If one doesn't exist, returns a new Invoice Item.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public DmoInvoiceItem Dispatch(GetInvoiceItemAction action)
    {
        var record = this._items.SingleOrDefault(item => item.Record.Id == action.id)?.Record
            ?? new DmoInvoiceItem { InvoiceId = this.InvoiceRecord.Record.Id, Id = InvoiceItemId.Create };

        return record;
    }
}
