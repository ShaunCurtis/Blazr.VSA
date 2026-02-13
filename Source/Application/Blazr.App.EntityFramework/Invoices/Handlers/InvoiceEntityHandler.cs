/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core.Invoices;

namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing record requests to get a Customer Entity in an Entity Framework Context
/// </summary>
public sealed class InvoiceEntityHandler : IRequestHandler<InvoiceEntityRequest, Result<InvoiceEntity>>
{
    private IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public InvoiceEntityHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Result<InvoiceEntity>> HandleAsync(InvoiceEntityRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        // Get the Invoice
        var invoiceResult = await dbContext
            .GetRecordFromDatastoreAsync<DvoInvoice>(new RecordQueryRequest<DvoInvoice>(item => item.InvoiceID == request.Id.Value))
            .BindAsync(DvoInvoice.MapToResult);

        // if we failed convert the result to the corrrect return type and exit.  This will pass through the error or exception
        if (invoiceResult.HasNotSucceeded)
            return invoiceResult.Convert(InvoiceEntityFactory.Create());

        // Get the invoice items associated with the invoice
        var invoiceItemsResult = await dbContext
            .GetItemsAsync(new ListQueryRequest<DvoInvoiceItem>()
                {
                    FilterExpression = item => item.InvoiceID == request.Id.Value, 
                })
            .MapAsync(provider => provider.Items.Select(item => DvoInvoiceItem.Map(item)));

        // if we failed convert the result to the corrrect return type and exit.  This will pass through the error or exception
        if (invoiceItemsResult.HasNotSucceeded)
            return invoiceResult.Convert(InvoiceEntityFactory.Create());

        // We have all we need now to build and invoice entity
        // We load it bypassing the entity rules.  The Mutor will take care of any updates required.
        var invoice = invoiceResult.Write<DmoInvoice>(DmoInvoice.CreateNew());
        var items = invoiceItemsResult.Write(Enumerable.Empty<DmoInvoiceItem>());
        var entity = InvoiceEntityFactory.Load(invoice, items);

        return ResultT.Read(entity);
    }
}
