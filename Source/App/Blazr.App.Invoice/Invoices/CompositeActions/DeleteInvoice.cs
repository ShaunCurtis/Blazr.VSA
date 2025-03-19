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
    public readonly record struct DeleteInvoiceAction();
}

public sealed partial class InvoiceComposite
{
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
}
