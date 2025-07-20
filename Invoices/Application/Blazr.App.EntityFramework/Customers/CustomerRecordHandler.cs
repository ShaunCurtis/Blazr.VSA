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
    private IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public CustomerRecordHandler(IDbContextFactory<InMemoryWeatherTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Result<DmoCustomer>> HandleAsync(CustomerRecordRequest request, CancellationToken cancellationToken)
    {
        var asyncResult = await _factory.CreateDbContext()
            .GetRecordAsync<DvoCustomer>(new RecordQueryRequest<DvoCustomer>(item => item.CustomerID == request.Id.Value));

        return asyncResult.MapToResult<DmoCustomer>(CustomerMap.Map);
    }
}
