/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.Antimony.Infrastructure.Server;
using Blazr.App.Invoice.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediatr Server Handler that builds an Invoice Aggregate
/// It uses the Antimony Database Handlers to interface with the database
/// </summary>
public sealed class InvoiceAggregateServerHandler : IRequestHandler<InvoiceRequests.InvoiceRequest, Result<InvoiceComposite>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public InvoiceAggregateServerHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<InvoiceComposite>> Handle(InvoiceRequests.InvoiceRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        DmoInvoice? invoice = null;
        
        {
            Expression<Func<DvoInvoice, bool>> findExpression = (item) =>
                item.InvoiceID == request.Id.Value;

            var query = new RecordQueryRequest<DvoInvoice>(findExpression);
            var result = await dbContext.GetRecordAsync<DvoInvoice>(query);
            
            if (!result.HasSucceeded(out DvoInvoice? record))
                return result.ConvertFail<InvoiceComposite>();
            
            invoice = InvoiceMap.Map(record);
        }

        List<DmoInvoiceItem>? invoiceItems = new();
        {
            Expression<Func<DboInvoiceItem, bool>> filterExpression = (item) =>
                item.InvoiceID == request.Id.Value;
            
            var query = new ListQueryRequest<DboInvoiceItem>() { FilterExpression=filterExpression };
            var result = await dbContext.GetItemsAsync<DboInvoiceItem>(query);
            
            if (!result.HasSucceeded(out ListItemsProvider<DboInvoiceItem>? records))
                return result.ConvertFail<InvoiceComposite>();
            
            invoiceItems = records.Items.Select(item => InvoiceItemMap.Map(item)).ToList();
        }

        var invoiceComposite = new InvoiceComposite(invoice, invoiceItems);

        return Result<InvoiceComposite>.Success(invoiceComposite);
    }
}
