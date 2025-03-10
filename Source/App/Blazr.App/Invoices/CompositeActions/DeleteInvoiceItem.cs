/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.InvoiceActions;

namespace Blazr.App.Core;
public static partial class InvoiceActions
{
    public readonly record struct DeleteInvoiceItemAction(InvoiceItemId Id);
}

/// <summary>
/// Contains all the actions that can be applied to the Invoice Aggregate
/// </summary>
public sealed partial class InvoiceComposite
{
    /// <summary>
    /// Moves an InvoiveItem to the Bin
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result Dispatch(DeleteInvoiceItemAction action)
    {
        var invoiceItem = this.Items.FirstOrDefault(item => item.Record.Id == action.Id);
        if (invoiceItem is null)
            return Result.Fail(new ActionException($"No Invoice Item with Id: {action.Id} exists in the Invoice"));

        // we don't set the Command State to delete because the handler needs to know
        // if the deleted item is New and therefore not in the data store
        // The fact that the item is in the Bin is enough to delete it.
        this.ItemsBin.Add(invoiceItem);
        this.Items.Remove(invoiceItem);
        this.Process();

        return Result.Success();
    }
}
