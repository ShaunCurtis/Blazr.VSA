/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.App.Invoice.Core;
using MediatR;

namespace Blazr.App.Invoice.Presentation;

public sealed class InvoiceCompositeBroker
{
    private readonly IMediator _dispatcher;

    public IResult LastResult { get; private set; } = Result.Success();

    public InvoiceComposite Invoice { get; private set; }

    public IQueryable<DmoInvoiceItem> InvoiceItems => this.Invoice.InvoiceItems.Select(item => item.Record).AsQueryable();

    public InvoiceCompositeBroker(IMediator mediator)
    {
        _dispatcher = mediator;

        // Get a default Invoice
        this.Invoice = InvoiceComposite.Default;
    }

    public async Task LoadAsync(InvoiceId id)
    {
        this.LastResult = Result.Success();

        // if we have an empty guid them we go with the new context created in the constructor
        if (id.Value != Guid.Empty)
        {
            var request = new InvoiceRequests.InvoiceRequest(id);
            var result = await _dispatcher.Send(request);

            LastResult = result;

            if (result.HasSucceeded(out InvoiceComposite? invoice))
                this.Invoice = invoice!;
        }
    }

    public void Reset()
    {
        this.LastResult = Result.Success();
        this.Invoice = InvoiceComposite.Default;
    }

    public async ValueTask<Result> SaveAsync()
    {
        var result = await _dispatcher.Send(new InvoiceRequests.InvoiceSaveRequest(this.Invoice));

        LastResult = result;

        if (result.IsFailure)
            return result;

        return this.Invoice.Dispatch(new InvoiceActions.SetAsPersistedAction());
    }

    public Result FakePersistenceToAllowExit()
    {
        return this.Invoice.Dispatch(new InvoiceActions.SetAsPersistedAction());
    }
}
