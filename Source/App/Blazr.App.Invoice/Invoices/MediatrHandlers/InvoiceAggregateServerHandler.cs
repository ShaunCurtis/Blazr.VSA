/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.App.Invoice.Core;
using MediatR;
using System.Linq.Expressions;

namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediatr Server Handler that builds an Invoice Aggregate
/// It uses the Antimony Database Handlers to interface with the database
/// </summary>
public sealed class InvoiceAggregateServerHandler : IRequestHandler<InvoiceRequests.InvoiceRequest, Result<InvoiceComposite>>
{
    private readonly IListRequestBroker _listBroker;
    private readonly IRecordRequestBroker _recordBroker;

    public InvoiceAggregateServerHandler(IRecordRequestBroker recordBroker, IListRequestBroker listBroker)
    {
        _listBroker = listBroker;
        _recordBroker = recordBroker;
    }

    public async Task<Result<InvoiceComposite>> Handle(InvoiceRequests.InvoiceRequest request, CancellationToken cancellationToken)
    {
        DmoInvoice? invoice = null;
        
        {
            Expression<Func<DvoInvoice, bool>> findExpression = (item) =>
                item.InvoiceID == request.Id.Value;

            var query = new RecordQueryRequest<DvoInvoice>(findExpression);
            var result = await _recordBroker.ExecuteAsync<DvoInvoice>(query);
            
            if (!result.HasSucceeded(out DvoInvoice? record))
                return result.ConvertFail<InvoiceComposite>();
            
            invoice = DvoInvoiceMap.Map(record);
        }

        List<DmoInvoiceItem>? invoiceItems = new();
        {
            Expression<Func<DboInvoiceItem, bool>> filterExpression = (item) =>
                item.InvoiceID == request.Id.Value;
            
            var query = new ListQueryRequest<DboInvoiceItem>() { FilterExpression=filterExpression };
            var result = await _listBroker.ExecuteAsync<DboInvoiceItem>(query);
            
            if (!result.HasSucceeded(out ListItemsProvider<DboInvoiceItem>? records))
                return result.ConvertFail<InvoiceComposite>();
            
            invoiceItems = records.Items.Select(item => DboInvoiceItemMap.Map(item)).ToList();
        }

        var invoiceComposite = new InvoiceComposite(invoice, invoiceItems);

        return Result<InvoiceComposite>.Success(invoiceComposite);
    }
}
