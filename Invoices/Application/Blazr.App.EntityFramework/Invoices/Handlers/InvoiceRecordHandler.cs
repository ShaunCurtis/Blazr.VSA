/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing record requests to get a Customer Entity in an Entity Framework Context
/// </summary>
public sealed class InvoiceRecordHandler : IRequestHandler<InvoiceEntityRequest, Return<InvoiceEntity>>
{
    private IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public InvoiceRecordHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Return<InvoiceEntity>> HandleAsync(InvoiceEntityRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        var invoiceResult = await dbContext
            .GetRecordAsync<DvoInvoice>(new RecordQueryRequest<DvoInvoice>(item => item.InvoiceID == request.Id.Value))
            .BindAsync(DvoInvoice.MapToBool);

        if (invoiceResult.HasException)
            return Return<InvoiceEntity>.Failure(invoiceResult.Exception!);

        var invoiceItemsResult = await dbContext
            .GetItemsAsync(new ListQueryRequest<DvoInvoiceItem>()
                {
                    FilterExpression = item => item.InvoiceID == request.Id.Value, 
                })
            .BindAsync(provider => provider.Items.Select(item => DvoInvoiceItem.Map(item)).ToBool());

        if (invoiceItemsResult.HasException)
            return Return<InvoiceEntity>.Failure(invoiceItemsResult.Exception!);

        return InvoiceEntity.CreateWithRulesValidation(invoiceResult.Value!, invoiceItemsResult.Value!);
    }
}
