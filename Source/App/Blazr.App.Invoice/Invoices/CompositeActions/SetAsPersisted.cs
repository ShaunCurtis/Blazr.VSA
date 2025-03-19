/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using static Blazr.App.Invoice.Core.InvoiceActions;

namespace Blazr.App.Invoice.Core;
public static partial class InvoiceActions
{
    public readonly record struct SetAsPersistedAction();
}

/// <summary>
/// Contains all the actions that can be applied to the Invoice Aggregate
/// </summary>
public sealed partial class InvoiceComposite
{
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

        foreach (var item in _items)
            item.State = CommandState.None;

        return Result.Success();
    }
}
