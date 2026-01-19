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
public sealed record InvoiceCommandHandler : IRequestHandler<InvoiceEntityCommandRequest, Result<InvoiceEntity>>
{
    private readonly IMessageBus _messageBus;
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;
    private readonly IRequestHandler<InvoiceEntityRequest, Result<InvoiceEntity>> _recordRequestHandler;
    private CancellationToken _cancellationToken;

    public InvoiceCommandHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory, IMessageBus messageBus, IRequestHandler<InvoiceEntityRequest, Result<InvoiceEntity>> requestHandler)
    {
        _factory = factory;
        _messageBus = messageBus;
        _recordRequestHandler = requestHandler;
    }

    public async Task<Result<InvoiceEntity>> HandleAsync(InvoiceEntityCommandRequest request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        var entity = request.Item;

        var recordRequest = new InvoiceEntityRequest(request.Item.InvoiceRecord.Id);

        // if the record exists in the datastore delete it
        var recordResult = await _recordRequestHandler.HandleAsync(recordRequest, cancellationToken)
            .BindAsync(this.DeleteEntityAsync);

        // Result if it was a delete command - everything is done
        if (request.State is RecordState.Deleted)
            return ResultT.Successful(entity);

        // We're either add or update so we add the record
        var addResult = (await this.AddEntityAsync(request.Item, cancellationToken))
            .Bind(Notify);

        return addResult;
    }

    private Result<InvoiceEntity> Notify(InvoiceEntity entity)
    {
        _messageBus.Publish<InvoiceEntity>(entity.InvoiceRecord.Id);
        return ResultT.Successful(entity);
    }

    private async Task<Result<InvoiceEntity>> DeleteEntityAsync(InvoiceEntity entity)
    {
        using var dbContext = _factory.CreateDbContext();

        dbContext.Remove<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));

        foreach (var invoiceItem in entity.InvoiceItems)
            dbContext.Remove<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));

        var addedItems = await dbContext.SaveChangesAsync(_cancellationToken);

        if (addedItems != entity.InvoiceItems.Count + 1)
            return Result<InvoiceEntity>.Failure("The Invoice was not added corectly.  Check the result.");

        return ResultT.Successful(entity);
    }

    private async Task<Result<InvoiceEntity>> AddEntityAsync(InvoiceEntity entity, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        dbContext.Add<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));

        foreach (var invoiceItem in entity.InvoiceItems)
            dbContext.Add<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));

        var addedItems = await dbContext.SaveChangesAsync(cancellationToken);

        if (addedItems != entity.InvoiceItems.Count + 1)
            return Result<InvoiceEntity>.Failure("The Invoice was not added corectly.  Check the result.");

        return ResultT.Successful(entity);
    }
}
