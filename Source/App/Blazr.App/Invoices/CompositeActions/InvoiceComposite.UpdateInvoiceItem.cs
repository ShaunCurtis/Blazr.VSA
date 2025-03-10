﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.InvoiceActions;

namespace Blazr.App.Core;
public static partial class InvoiceActions
{
    public readonly record struct UpdateInvoiceItemAction(DmoInvoiceItem Item);
}

/// <summary>
/// Contains all the actions that can be applied to the Invoice Aggregate
/// </summary>
public sealed partial class InvoiceComposite
{

    /// <summary>
    /// Updates an existing InvoiceItem in the Invoice
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result Dispatch(UpdateInvoiceItemAction action)
    {
        var invoiceItem = this.Items.FirstOrDefault(item => item.Record.Id.Equals(action.Item.Id));

        if (invoiceItem is null)
            return Result.Fail(new ActionException($"No Invoice Item with Id: {action.Item.Id} exists in the Invoice."));

        invoiceItem.State = invoiceItem.State.AsDirty;
        invoiceItem.Update(action.Item);
        this.Process();

        return Result.Success();
    }
}
