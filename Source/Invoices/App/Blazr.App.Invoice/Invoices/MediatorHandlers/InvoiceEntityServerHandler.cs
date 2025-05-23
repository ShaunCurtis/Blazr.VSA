/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.Antimony.Infrastructure.EntityFramework;
using Blazr.App.Invoice.Core;
using Blazr.Antimony.Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediatr Server Handler that builds an Invoice Composite
/// It uses the Antimony DBContext CQS extensions to interface with the database
/// </summary>
public sealed class InvoiceEntityServerHandler : IRequestHandler<InvoiceRequests.InvoiceRequest, Result<InvoiceEntity>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;
    private readonly IMediatorBroker _mediator;

    public InvoiceEntityServerHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory, IMediatorBroker mediator)
    {
        _factory = factory;
        _mediator = mediator;
    }

    public async Task<Result<InvoiceEntity>> HandleAsync(InvoiceRequests.InvoiceRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        DmoInvoice? invoice = null;
        
        {
            // Define the predicate expression to get the Invoice record
            Expression<Func<DvoInvoice, bool>> findExpression = (item) =>
                item.InvoiceID == request.Id.Value;

            // define the query and execute the DBContext extension
            var query = new RecordQueryRequest<DvoInvoice>(findExpression);
            var result = await dbContext.GetRecordAsync<DvoInvoice>(query);
            
            // Return an error result if we can't get the record
            if (!result.HasSucceeded(out DvoInvoice? record))
                return result.ConvertFail<InvoiceEntity>();
            
            // Convert to the domain object
            invoice = record.ToDmo();
        }

        List<DmoInvoiceItem>? invoiceItems = new();
        {
            // Define the predicate expression to get the Invoice Item records
            Expression<Func<DboInvoiceItem, bool>> filterExpression = (item) =>
                item.InvoiceID == request.Id.Value;

            // define the query and execute the DBContext extension
            var query = new ListQueryRequest<DboInvoiceItem>() { FilterExpression=filterExpression };
            var result = await dbContext.GetItemsAsync<DboInvoiceItem>(query);

            // Return an error result if the query failed
            if (!result.HasSucceeded(out ListItemsProvider<DboInvoiceItem>? records))
                return result.ConvertFail<InvoiceEntity>();

            // Convert to the domain object
            invoiceItems = records.Items.Select(item => item.ToDmo()).ToList();
        }

        // Build the new composite
        var invoiceComposite = new InvoiceEntity(_mediator, invoice, invoiceItems);

        return Result<InvoiceEntity>.Success(invoiceComposite);
    }
}
