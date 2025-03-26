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

    public InvoiceEntity Invoice { get; private set; } = default!;

    public IQueryable<DmoInvoiceItem> InvoiceItems => this.Invoice.InvoiceItems.Select(item => item.Record).AsQueryable();

    public InvoiceCompositeBroker(IMediator mediator)
    {
        _dispatcher = mediator;
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

            if (result.HasSucceeded(out InvoiceEntity? invoice))
                this.Invoice = invoice!;
        }
    }

    public void Reset()
    {
        this.LastResult = Result.Success();
        this.Invoice.ResetInvoice();
    }

    public async ValueTask<Result> SaveAsync()
    {
        var result = await this.Invoice.PersistInvoiceAsync();

        LastResult = result;

        return result;
    }

    public void AllowExit()
    {
        this.Invoice.ResetInvoice();
    }
}
