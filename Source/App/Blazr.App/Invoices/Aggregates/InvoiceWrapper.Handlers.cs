/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.InvoiceActions;

namespace Blazr.App.Core;

/// <summary>
/// Contains all the actions that can be applied to the Invoice Aggregate
/// </summary>
public sealed partial class InvoiceWrapper
{
    /// <summary>
    /// Updates the Invoice record
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result Dispatch(UpdateInvoiceAction action)
    {
        this.Invoice.Update(action.Item);
        return Result.Success();
    }

    /// <summary>
    /// Marks the invoice for deletion
    /// You still need to persist the change to the data store
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Result Dispatch(DeleteInvoiceAction action)
    {
        this.Invoice.State = CommandState.Delete;
        return Result.Success();
    }

    /// <summary>
    /// Gets an existing Invoice Item.  If one doesn't exist, reurns a new Invoice Item.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public DmoInvoiceItem Dispatch(GetInvoiceItemAction action)
    {
        var record = this.Items.SingleOrDefault(item => item.Record.Id == action.id);

        return record?.Record
            ?? new DmoInvoiceItem { InvoiceId = this.InvoiceRecord.Record.Id, Id = InvoiceItemId.Create };
    }

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

    /// <summary>
    /// Adds an InvoiceItem to the Invoice
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result Dispatch(AddInvoiceItemAction action)
    {
        if (this.Items.Any(item => item.Record == action.Item))
            return Result.Fail(new ActionException($"The Invoice Item with Id: {action.Item.Id} already exists in the Invoice."));

        var invoiceItemRecord = action.Item with { InvoiceId = this.InvoiceRecord.Record.Id };
        var invoiceItem = new InvoiceItem(invoiceItemRecord, this.ItemUpdated, action.IsNew);
        this.Items.Add(invoiceItem);
        this.Process();

        return Result.Success();
    }

    /// <summary>
    /// Sets the aggregate as saved.
    /// i.e. it sets the CommandState on the invoice and invoice items as none. 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Result Dispatch(SetAsPersistedAction action)
    {
        this.Invoice.State = CommandState.None;

        foreach (var item in this.Items)
            item.State = CommandState.None;

        return Result.Success();
    }
}
