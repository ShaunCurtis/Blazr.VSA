/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core.Invoices;

namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing commands against a Invoice Entity in an Entity Framework Context
/// 
/// The basic premis is:
///  - Get the entity data set
///  - Delete the whole data set if it exists
///  - Add the data set if we are adding or updating 
/// </summary>
public sealed record InvoiceCommandHandler : IRequestHandler<InvoiceEntityCommandRequest, Return>
{
    private readonly IMessageBus _messageBus;
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;
    private readonly IRequestHandler<InvoiceEntityRequest, Return<InvoiceEntity>> _recordRequestHandler;
    private CancellationToken _cancellationToken;

    public InvoiceCommandHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory, IMessageBus messageBus, IRequestHandler<InvoiceEntityRequest, Return<InvoiceEntity>> requestHandler)
    {
        _factory = factory;
        _messageBus = messageBus;
        _recordRequestHandler = requestHandler;
    }

    public async Task<Return> HandleAsync(InvoiceEntityCommandRequest request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var recordRequest = new InvoiceEntityRequest(request.Item.InvoiceRecord.Id);

        // if the record exists in the datastore delete it
        var recordResult = await _recordRequestHandler.HandleAsync(recordRequest, cancellationToken)
            .BindAsync(this.DeleteEntityAsync);

        // Return if it was a delet command - everything is done
        if (request.State is RecordState.Deleted)
            return Return.Success();

        // We're either add or update so we add the record
        var addResult = await this.AddEntityAsync(request.Item, cancellationToken)
            .NotifyAsync((entity) => _messageBus.Publish<InvoiceEntity>(entity.InvoiceRecord.Id));

        return addResult.ToReturn();
    }

    private async Task<Return<InvoiceEntity>> DeleteEntityAsync(InvoiceEntity entity)
    {
        using var dbContext = _factory.CreateDbContext();

        dbContext.Remove<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));

        foreach (var invoiceItem in entity.InvoiceItems)
            dbContext.Remove<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));

        var addedItems = await dbContext.SaveChangesAsync(_cancellationToken);

        if (addedItems != entity.InvoiceItems.Count + 1)
            return Return<InvoiceEntity>.Failure("The Invoice was not added corectly.  Check the result.");

        return ReturnT.Success(entity);
    }

    private async Task<Return<InvoiceEntity>> AddEntityAsync(InvoiceEntity entity, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        dbContext.Add<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));

        foreach (var invoiceItem in entity.InvoiceItems)
            dbContext.Add<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));

        var addedItems = await dbContext.SaveChangesAsync(cancellationToken);

        if (addedItems != entity.InvoiceItems.Count + 1)
            return Return<InvoiceEntity>.Failure("The Invoice was not added corectly.  Check the result.");

        return ReturnT.Success(entity);
    }
}
