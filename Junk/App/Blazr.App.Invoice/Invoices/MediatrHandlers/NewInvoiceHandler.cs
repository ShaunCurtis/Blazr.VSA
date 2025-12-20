/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.App.Core;
using Blazr.Antimony.Mediator;

namespace Blazr.App.Invoice.Core;

/// <summary>
/// Mediatr Handler to return a new Invoice 
/// </summary>
public record NewInvoiceHandler : IRequestHandler<InvoiceRequests.InvoiceNewRequest, Result<InvoiceEntity>>
{
    private IEntityProvider<DmoInvoice, InvoiceId> _entityProvider;
    private IMediatorBroker _mediator;

    public NewInvoiceHandler(IEntityProvider<DmoInvoice, InvoiceId> entityProvider, IMediatorBroker mediator)
    {
        _mediator = mediator;
        _entityProvider = entityProvider;
    }

    public Task<Result<InvoiceEntity>> HandleAsync(InvoiceRequests.InvoiceNewRequest request, CancellationToken cancellationToken)
    {
        var invoiceRecord = _entityProvider.NewRecord;

        var invoiceComposite = new InvoiceEntity(_mediator, invoiceRecord, Enumerable.Empty<DmoInvoiceItem>());

        return Task.FromResult(Result<InvoiceEntity>.Success((InvoiceEntity)invoiceComposite));
    }
}
