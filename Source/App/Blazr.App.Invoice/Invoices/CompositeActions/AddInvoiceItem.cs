/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.App.Core;

using static Blazr.App.Invoice.Core.InvoiceActions;

namespace Blazr.App.Invoice.Core;

public static partial class InvoiceActions
{
    public readonly record struct AddInvoiceItemAction(DmoInvoiceItem Item, bool IsNew = true);
}

public sealed partial class InvoiceComposite
{
    /// <summary>
    /// Adds an InvoiceItem to the Invoice
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result Dispatch(AddInvoiceItemAction action)
    {
        if (_items.Any(item => item.Record == action.Item))
            return Result.Fail(new ActionException($"The Invoice Item with Id: {action.Item.Id} already exists in the Invoice."));

        var invoiceItemRecord = action.Item with { InvoiceId = this.InvoiceRecord.Record.Id };
        var invoiceItem = new InvoiceItem(invoiceItemRecord, action.IsNew);
        _items.Add(invoiceItem);
        this.Process();

        return Result.Success();
    }
}
