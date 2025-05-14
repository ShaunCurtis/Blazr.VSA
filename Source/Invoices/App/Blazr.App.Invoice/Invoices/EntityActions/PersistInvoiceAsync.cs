/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;

namespace Blazr.App.Invoice.Core;

public sealed partial class InvoiceEntity
{
    /// <summary>
    /// Persists the Composite to the data store and sets it as saved.
    /// i.e. it sets the CommandState on the invoice and invoice items as none. 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<Result> PersistInvoiceAsync(CancellationToken cancellationToken = new())
    {
        var result = await _mediator.Send(new InvoiceRequests.InvoiceSaveRequest(this), cancellationToken);

        if (result.IsFailure)
            return result;

        this.Invoice.State = CommandState.None;

        foreach (var item in _items)
            item.State = CommandState.None;

        return Result.Success();
    }
}
