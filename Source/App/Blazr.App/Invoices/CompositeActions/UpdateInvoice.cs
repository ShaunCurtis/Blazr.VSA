/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.InvoiceActions;

namespace Blazr.App.Core;

public static partial class InvoiceActions
{
    public readonly record struct UpdateInvoiceAction(DmoInvoice Item);
}

public sealed partial class InvoiceComposite
{
    /// <summary>
    /// Updates the Invoice record
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result Dispatch(UpdateInvoiceAction action)
    {
        this.Invoice.Update(action.Item);
        this.InvoiceUpdated();
        return Result.Success();
    }
}
