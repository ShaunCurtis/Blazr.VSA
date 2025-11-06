/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing commands against a Invoice Entity in an Entity Framework Context
/// 
/// The basic premis is:
///  - Get the entity data set
///  - Delete the whole data set if it exists
///  - Add the data set if we are adding or updating 
/// </summary>
public sealed record InvoiceCommandHandler : IRequestHandler<InvoiceCommandRequest, Result>
{
    private readonly IMessageBus _messageBus;
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;
    private readonly IRequestHandler<InvoiceRecordRequest, Result<InvoiceEntity>> _recordRequestHandler;

    public InvoiceCommandHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory, IMessageBus messageBus, IRequestHandler<InvoiceRecordRequest, Result<InvoiceEntity>> requestHandler)
    {
        _factory = factory;
        _messageBus = messageBus;
        _recordRequestHandler = requestHandler;
    }

    public async Task<Result> HandleAsync(InvoiceCommandRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);

        var recordResult = await _recordRequestHandler.HandleAsync(new InvoiceRecordRequest(request.Item.InvoiceRecord.Id), cancellationToken);

        if (recordResult.HasValue)
        {
            var deleteResult = await this.DeleteEntityAsync(recordResult.Value!, cancellationToken);

            if (deleteResult.Failed)
                return deleteResult;

            if (request.State == EditState.Deleted)
                return Result.Success();
        }

        var addResult = await this.AddEntityAsync(request.Item, cancellationToken);

        if (addResult.Failed)
            return addResult;

        _messageBus.Publish<InvoiceEntity>(request.Item.InvoiceRecord.Id);

        return Result.Success();
    }

    private async Task<Result> DeleteEntityAsync(InvoiceEntity entity, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        dbContext.Remove<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));

        foreach (var invoiceItem in entity.InvoiceItems)
            dbContext.Remove<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));

        var addedItems = await dbContext.SaveChangesAsync(cancellationToken);

        if (addedItems != entity.InvoiceItems.Count + 1)
            return Result.Failure("The Invoice was not added corectly.  Check the result.");

        return Result.Success();
    }

    private async Task<Result> AddEntityAsync(InvoiceEntity entity, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        dbContext.Add<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));

        foreach (var invoiceItem in entity.InvoiceItems)
            dbContext.Add<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));

        var addedItems = await dbContext.SaveChangesAsync(cancellationToken);

        if (addedItems != entity.InvoiceItems.Count + 1)
            return Result.Failure("The Invoice was not added corectly.  Check the result.");

        return Result.Success();
    }
}
