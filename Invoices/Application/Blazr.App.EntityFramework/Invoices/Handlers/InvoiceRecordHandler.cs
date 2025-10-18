/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing record requests to get a Customer Entity in an Entity Framework Context
/// </summary>
public sealed class InvoiceRecordHandler : IRequestHandler<InvoiceRecordRequest, Result<DroInvoice>>
{
    private IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public InvoiceRecordHandler(IDbContextFactory<InMemoryWeatherTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Result<DroInvoice>> HandleAsync(InvoiceRecordRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        var invoiceResult = await dbContext
            .GetRecordAsync<DvoInvoice>(new RecordQueryRequest<DvoInvoice>(item => item.InvoiceID == request.Id.Value))
            .ExecuteTransformAsync(DvoInvoice.MapToResult);

        if (invoiceResult.HasException)
            return Result<DroInvoice>.Failure(invoiceResult.Exception!);

        var invoiceItemsResult = await dbContext
            .GetItemsAsync(new ListQueryRequest<DvoInvoiceItem>()
                {
                    FilterExpression = item => item.InvoiceID == request.Id.Value, 
                })
            .ExecuteTransformAsync(provider => provider.Items.Select(item => DvoInvoiceItem.Map(item)).ToResult());

        if (invoiceItemsResult.HasException)
            return Result<DroInvoice>.Failure(invoiceItemsResult.Exception!);

        return DroInvoice.CreateAsResult(invoiceResult.Value!, invoiceItemsResult.Value!);
    }
}
