/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediatr Server Handler that builds an Invoice Aggregate
/// It uses the Antimony Database Handlers to interface with the database
/// </summary>
public sealed class InvoiceAggregateServerHandler : IRequestHandler<InvoiceRequests.InvoiceRequest, Result<InvoiceWrapper>>
{
    private readonly IListRequestBroker _listBroker;
    private readonly IRecordRequestBroker _recordBroker;

    public InvoiceAggregateServerHandler(IRecordRequestBroker recordBroker, IListRequestBroker listBroker)
    {
        _listBroker = listBroker;
        _recordBroker = recordBroker;
    }

    public async Task<Result<InvoiceWrapper>> Handle(InvoiceRequests.InvoiceRequest request, CancellationToken cancellationToken)
    {
        DmoInvoice? invoice = null;
        
        {
            Expression<Func<DvoInvoice, bool>> findExpression = (item) =>
                item.InvoiceID == request.Id.Value;

            var query = new RecordQueryRequest<DvoInvoice>(findExpression);
            var result = await _recordBroker.ExecuteAsync<DvoInvoice>(query);
            
            if (!result.HasSucceeded(out DvoInvoice? record))
                return result.ConvertFail<InvoiceWrapper>();
            
            invoice = DvoInvoiceMap.Map(record);
        }

        List<DmoInvoiceItem>? invoiceItems = new();
        {
            Expression<Func<DboInvoiceItem, bool>> filterExpression = (item) =>
                item.InvoiceID == request.Id.Value;
            
            var query = new ListQueryRequest<DboInvoiceItem>() { FilterExpression=filterExpression };
            var result = await _listBroker.ExecuteAsync<DboInvoiceItem>(query);
            
            if (!result.HasSucceeded(out ListResult<DboInvoiceItem> records))
                return result.ConvertFail<InvoiceWrapper>();
            
            invoiceItems = records.Items.Select(item => DboInvoiceItemMap.Map(item)).ToList();
        }

        var invoiceComposite = new InvoiceWrapper(invoice, invoiceItems);

        return Result<InvoiceWrapper>.Success(invoiceComposite);
    }
}
