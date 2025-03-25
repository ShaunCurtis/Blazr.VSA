/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.App.Invoice.Core;
using Blazr.Gallium;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediatr Server Handler that saves an Invoice Aggregate
/// It uses the custom Invoice Database Handler to interface with the database
/// </summary>
public sealed class PersistInvoiceServerHandler : IRequestHandler<InvoiceRequests.InvoiceSaveRequest, Result>
{
    private readonly IMessageBus _messageBus;
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public PersistInvoiceServerHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory, IMessageBus messageBus)
    {
        _factory = factory;
        _messageBus = messageBus;
    }

    public async Task<Result> Handle(InvoiceRequests.InvoiceSaveRequest request, CancellationToken cancellationToken)
    {
        var invoice = request.Invoice;

        using var dbContext = _factory.CreateDbContext();

        var invoiceRecord = InvoiceMap.Map(invoice.InvoiceRecord.Record);

        if (invoice.InvoiceRecord.State == CommandState.Update)
            dbContext.Update<DboInvoice>(invoiceRecord);

        if (invoice.InvoiceRecord.State == CommandState.Add)
            dbContext.Add<DboInvoice>(invoiceRecord);

        if (invoice.InvoiceRecord.State == CommandState.Delete)
            dbContext.Remove<DboInvoice>(invoiceRecord);

        foreach (var invoiceItem in invoice.InvoiceItems)
        {
            var invoiceItemRecord = InvoiceItemMap.Map(invoiceItem.Record);

            if (invoiceItem.State == CommandState.Update)
                dbContext.Update<DboInvoiceItem>(invoiceItemRecord);

            if (invoiceItem.State == CommandState.Add)
                dbContext.Add<DboInvoiceItem>(invoiceItemRecord);
        }

        foreach (var invoiceItem in invoice.InvoiceItemsBin)
        {
            if (invoiceItem.State != CommandState.Add)
            {
                var invoiceItemRecord = InvoiceItemMap.Map(invoiceItem.Record);

                dbContext.Remove<DboInvoiceItem>(invoiceItemRecord);
            }
        }

        var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        if (result < 1)
            return Result.Fail(new CommandException("Error adding Invoice Composite to the data store."));

        _messageBus.Publish<DmoInvoice>(invoice.InvoiceRecord.Record.Id);

        return Result.Success();
    }
}
