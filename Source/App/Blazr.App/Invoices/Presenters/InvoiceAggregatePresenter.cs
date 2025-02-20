/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public sealed class InvoiceAggregatePresenter
{
    private readonly IMediator _dispatcher;

    public IDataResult LastResult { get; private set; } = DataResult.Success();

    public InvoiceWrapper Invoice { get; private set; }

    public IQueryable<DmoInvoiceItem> InvoiceItems => this.Invoice.InvoiceItems.Select(item => item.Record).AsQueryable();

    public InvoiceAggregatePresenter(IMediator mediator)
    {
        _dispatcher = mediator;

        // Get a default Invoice
        this.Invoice = InvoiceWrapper.Default;
    }

    public async Task LoadAsync(InvoiceId id)
    {
        this.LastResult = DataResult.Success();

        // if we have an empty guid them we go with the new context created in the constructor
        if (id.Value != Guid.Empty)
        {
            var request = new InvoiceRequests.InvoiceRequest(id);
            var result = await _dispatcher.Send(request);

            LastResult = result.ToDataResult;

            if (result.HasSucceeded(out InvoiceWrapper? invoice))
                this.Invoice = invoice!;
        }
    }

    public void Reset()
    {
        this.LastResult = DataResult.Success();
        this.Invoice = InvoiceWrapper.Default;
    }

    public async ValueTask<Result> SaveAsync()
    {
        var result = await _dispatcher.Send(new InvoiceRequests.InvoiceSaveRequest(this.Invoice));

        LastResult = result.ToDataResult;

        if (result.IsFailure)
            return result;

        return this.Invoice.Dispatch(new InvoiceActions.SetAsPersistedAction());
    }

    public Result FakePersistenceToAllowExit()
    {
        return this.Invoice.Dispatch(new InvoiceActions.SetAsPersistedAction());
    }
}
