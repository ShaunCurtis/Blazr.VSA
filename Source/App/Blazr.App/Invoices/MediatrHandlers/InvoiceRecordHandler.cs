/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediator Handler to get an Invoice Entity - A DmoInvoice object
/// </summary>
public sealed class InvoiceRecordHandler : IRequestHandler<InvoiceRequests.InvoiceRecordRequest, Result<DmoInvoice>>
{
    private IRecordRequestBroker _broker;

    public InvoiceRecordHandler(IRecordRequestBroker broker)
    {
        _broker = broker;
    }

    public async Task<Result<DmoInvoice>> Handle(InvoiceRequests.InvoiceRecordRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<DvoInvoice, bool>> findExpression = (item) =>
            item.InvoiceID == request.Id.Value;

        var query = new RecordQueryRequest<DvoInvoice>(findExpression);

        var result = await _broker.ExecuteAsync<DvoInvoice>(query);

        if (!result.HasSucceeded(out DvoInvoice? record))
            return result.ConvertFail<DmoInvoice>();

        var returnItem = DvoInvoiceMap.Map(record);

        return Result<DmoInvoice>.Success(returnItem);
    }
}
