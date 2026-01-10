/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing record requests to get a Customer Entity in an Entity Framework Context
/// </summary>
public sealed class CustomerRecordHandler : IRequestHandler<CustomerRecordRequest, Result<DmoCustomer>>
{
    private IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerRecordHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> dbContextFactory)
        => _factory = dbContextFactory;

    public async Task<Result<DmoCustomer>> HandleAsync(CustomerRecordRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        return await dbContext
            .GetRecordFromDatastoreAsync<DvoCustomer>(new RecordQueryRequest<DvoCustomer>(item => item.CustomerID == request.Id.Value))
            .MapAsync(DvoCustomer.MapToDomainEntity);
    }
}
