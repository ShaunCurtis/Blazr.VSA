/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using static Blazr.App.Invoice.Core.InvoiceActions;

namespace Blazr.App.Invoice.Core;
public static partial class InvoiceActions
{
    public readonly record struct PersistInvoiceAction();
}

public sealed partial class InvoiceEntity
{
    /// <summary>
    /// Persists the Composite to the data store and sets it as saved.
    /// i.e. it sets the CommandState on the invoice and invoice items as none. 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<Result> DispatchAsync(PersistInvoiceAction action, CancellationToken cancellationToken = new())
    {
        return PersistInvoiceAsync(cancellationToken);
    }

    /// <summary>
    /// Persists the Composite to the data store and sets it as saved.
    /// i.e. it sets the CommandState on the invoice and invoice items as none. 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<Result> PersistInvoiceAsync(CancellationToken cancellationToken = new())
    {
        var result = await _mediator.DispatchAsync(new InvoiceRequests.InvoiceSaveRequest(this), cancellationToken);

        if (result.IsFailure)
            return result;

        this.Invoice.State = CommandState.None;

        foreach (var item in _items)
            item.State = CommandState.None;

        return Result.Success();
    }
}
