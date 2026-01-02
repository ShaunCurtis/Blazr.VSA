/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing commands against a Invoice Entity in an Entity Framework Context
/// 
/// The basic premis is either:
///  - Delete the whole data set if marked for deletion
///  or
///  - get a list of all the invoice items in the database
///  - Add or update items in the Entity based on their ID state
///  - remove each item from the all items list
///  - remove any remaining items in th all items list from the database
/// </summary>
public sealed record AltInvoiceCommandHandler : IRequestHandler<InvoiceCommandRequest, Return>
{
    private readonly IMessageBus _messageBus;
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;
    private readonly IRequestHandler<InvoiceEntityRequest, Return<InvoiceEntity>> _recordRequestHandler;

    public AltInvoiceCommandHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory, IMessageBus messageBus, IRequestHandler<InvoiceEntityRequest, Return<InvoiceEntity>> requestHandler)
    {
        _factory = factory;
        _messageBus = messageBus;
        _recordRequestHandler = requestHandler;
    }

    public async Task<Return> HandleAsync(InvoiceCommandRequest request, CancellationToken cancellationToken)
    {
        if (request.State.IsMarkedForDeletion)
            return await this.DeleteEntityAsync(request.Item.InvoiceRecord.Id, cancellationToken);

        return await this.SaveEntityAsync(request.Item, cancellationToken);
    }

    private async Task<Return> DeleteEntityAsync(InvoiceId id, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        var recordResult = await _recordRequestHandler.HandleAsync(new InvoiceEntityRequest(id), cancellationToken);

        if (recordResult.HasException)
            return recordResult.ToReturn();

        var entity = recordResult.Value!;

        dbContext.Remove<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));

        foreach (var invoiceItem in entity.InvoiceItems)
            dbContext.Remove<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));

        var addedItems = await dbContext.SaveChangesAsync(cancellationToken);

        if (addedItems != entity.InvoiceItems.Count + 1)
            return Return.Failure("The Invoice was not added corectly.  Check the result.");

        return Return.Success();
    }

    private async Task<Return> SaveEntityAsync(InvoiceEntity entity, CancellationToken cancellationToken)
    {
        var itemList = await this.GetInvoiceItemsAsync(entity.InvoiceRecord.Id);

        using var dbContext = await _factory.CreateDbContextAsync();

        if (entity.InvoiceRecord.Id.IsNew)
            dbContext.Add<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));
        else
            dbContext.Update<DboInvoice>(DboInvoice.Map(entity.InvoiceRecord));

        int transactionCount = 1;

        foreach (var invoiceItem in entity.InvoiceItems)
        {
            if (invoiceItem.Id.IsNew)
                dbContext.Add<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));
            else
            {
                dbContext.Update<DboInvoiceItem>(DboInvoiceItem.Map(invoiceItem));

                var currentItem = itemList.First(item => item.InvoiceItemID == invoiceItem.Id.Value);
                itemList.Remove(currentItem);
            }
            transactionCount++;
        }

        foreach (var invoiceItem in itemList)
        {
            dbContext.Remove<DboInvoiceItem>(invoiceItem);
            transactionCount++;
        }

        var addedItems = await dbContext.SaveChangesAsync(cancellationToken);

        if (addedItems != transactionCount)
            return Return.Failure("The Invoice was not added correctly.  Check the result.");

        return Return.Success();
    }

    private async Task<List<DboInvoiceItem>> GetInvoiceItemsAsync(InvoiceId id)
    {
        using var dbContext = await _factory.CreateDbContextAsync();

        var itemList = await dbContext.InvoiceItems.Where(item => item.InvoiceID == id.Value).ToListAsync();
        return itemList;
    }
}
