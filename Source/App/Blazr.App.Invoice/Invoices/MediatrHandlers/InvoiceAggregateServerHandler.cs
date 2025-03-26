/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.Antimony.Infrastructure.EntityFramework;
using Blazr.App.Invoice.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediatr Server Handler that builds an Invoice Aggregate
/// It uses the Antimony Database Handlers to interface with the database
/// </summary>
public sealed class InvoiceAggregateServerHandler : IRequestHandler<InvoiceRequests.InvoiceRequest, Result<InvoiceEntity>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;
    private readonly IMediator _mediator;

    public InvoiceAggregateServerHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory, IMediator mediator)
    {
        _factory = factory;
        _mediator = mediator;
    }

    public async Task<Result<InvoiceEntity>> Handle(InvoiceRequests.InvoiceRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        DmoInvoice? invoice = null;
        
        {
            Expression<Func<DvoInvoice, bool>> findExpression = (item) =>
                item.InvoiceID == request.Id.Value;

            var query = new RecordQueryRequest<DvoInvoice>(findExpression);
            var result = await dbContext.GetRecordAsync<DvoInvoice>(query);
            
            if (!result.HasSucceeded(out DvoInvoice? record))
                return result.ConvertFail<InvoiceEntity>();
            
            invoice = InvoiceMap.Map(record);
        }

        List<DmoInvoiceItem>? invoiceItems = new();
        {
            Expression<Func<DboInvoiceItem, bool>> filterExpression = (item) =>
                item.InvoiceID == request.Id.Value;
            
            var query = new ListQueryRequest<DboInvoiceItem>() { FilterExpression=filterExpression };
            var result = await dbContext.GetItemsAsync<DboInvoiceItem>(query);
            
            if (!result.HasSucceeded(out ListItemsProvider<DboInvoiceItem>? records))
                return result.ConvertFail<InvoiceEntity>();
            
            invoiceItems = records.Items.Select(item => InvoiceItemMap.Map(item)).ToList();
        }

        var invoiceComposite = new InvoiceEntity(_mediator, invoice, invoiceItems);

        return Result<InvoiceEntity>.Success(invoiceComposite);
    }
}
