/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Invoice.Core.InvoiceActions;
using Blazr.Antimony;
using Blazr.App.Core;

namespace Blazr.App.Invoice.Core;
public static partial class InvoiceActions
{
    public readonly record struct DeleteInvoiceItemAction(InvoiceItemId Id);
}

/// <summary>
/// Contains all the actions that can be applied to the Invoice Aggregate
/// </summary>
public sealed partial class InvoiceEntity
{
    /// <summary>
    /// Moves an InvoiveItem to the Bin
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result Dispatch(DeleteInvoiceItemAction action)
    {
        var invoiceItem = this._items.FirstOrDefault(item => item.Record.Id == action.Id);
        if (invoiceItem is null)
            return Result.Fail(new ActionException($"No Invoice Item with Id: {action.Id} exists in the Invoice"));

        // we don't set the Command State to delete because the handler needs to know
        // if the deleted item is New and therefore not in the data store
        // The fact that the item is in the Bin is enough to delete it.
        _itemsBin.Add(invoiceItem);
        _items.Remove(invoiceItem);
        this.ApplyRules();

        return Result.Success();
    }
}
