/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Broker implements the standard Server Command Handler against the EF `TDbContext`
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public sealed class InvoiceCommandServerBroker<TDbContext>
    : ICommandBroker<InvoiceWrapper>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public InvoiceCommandServerBroker(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<Result<InvoiceWrapper>> ExecuteAsync(CommandRequest<InvoiceWrapper> request)
    {
        return await this.ExecuteCommandAsync(request);
    }

    private async ValueTask<Result<InvoiceWrapper>> ExecuteCommandAsync(CommandRequest<InvoiceWrapper> request)
    {
        var invoice = request.Item;

        using var dbContext = _factory.CreateDbContext();

        var invoiceRecord = DboInvoiceMap.Map(request.Item.InvoiceRecord.Record);

        if (request.Item.InvoiceRecord.State == CommandState.Update)
            dbContext.Update<DboInvoice>(invoiceRecord);

        if (request.Item.InvoiceRecord.State == CommandState.Add)
            dbContext.Add<DboInvoice>(invoiceRecord);

        if (request.Item.InvoiceRecord.State == CommandState.Delete)
            dbContext.Remove<DboInvoice>(invoiceRecord);

        foreach (var invoiceItem in request.Item.InvoiceItems)
        {
            var invoiceItemRecord = DboInvoiceItemMap.Map(invoiceItem.Record);

            if (invoiceItem.State == CommandState.Update)
                dbContext.Update<DboInvoiceItem>(invoiceItemRecord);

            if (invoiceItem.State == CommandState.Add)
                dbContext.Add<DboInvoiceItem>(invoiceItemRecord);
        }

        foreach (var invoiceItem in request.Item.InvoiceItemsBin)
        {
            if (invoiceItem.State != CommandState.Add)
            {
                var invoiceItemRecord = DboInvoiceItemMap.Map(invoiceItem.Record);

                dbContext.Remove<DboInvoiceItem>(invoiceItemRecord);
            }
        }

        var result = await dbContext.SaveChangesAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None);

        return result > 0
            ? Result<InvoiceWrapper>.Success(invoice)
            : Result<InvoiceWrapper>.Fail(new CommandException("Error adding Invoice Composite to the data store."));
    }
}
