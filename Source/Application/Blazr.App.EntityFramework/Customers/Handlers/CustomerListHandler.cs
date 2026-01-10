using Azure.Core;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing list requests against a Customer Entity in a Entity Framework Context
/// </summary>
public sealed class CustomerListHandler : IRequestHandler<CustomerListRequest, Result<ListItemsProvider<DmoCustomer>>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerListHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
        => _factory = factory;

    public async Task<Result<ListItemsProvider<DmoCustomer>>> HandleAsync(CustomerListRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        return await dbContext
            .GetItemsAsync<DvoCustomer>(GetListRequest(request, cancellationToken))
            .MapAsync(MapToDomainEntitiyListProvider);
    }

    private ListQueryRequest<DvoCustomer> GetListRequest(CustomerListRequest request, CancellationToken cancellationToken)
        => new ListQueryRequest<DvoCustomer>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

    private static ListItemsProvider<DmoCustomer> MapToDomainEntitiyListProvider(ListItemsProvider<DvoCustomer> provider)
        => new ListItemsProvider<DmoCustomer>(
            Items: provider.Items.Select(item => DvoCustomer.MapToDomainEntity(item)),
            TotalCount: provider.TotalCount);

    private Expression<Func<DvoCustomer, object>> GetSorter(string? field)
        => field switch
        {
            "Id" => (Item) => Item.CustomerID,
            "ID" => (Item) => Item.CustomerID,
            "Name" => (Item) => Item.CustomerName ?? "No Customer Name Set",
            _ => (item) => item.CustomerID // Default Sort by ID
        };

    // No Filter Defined
    private Expression<Func<DvoCustomer, bool>>? GetFilter(CustomerListRequest request)
    {
        return null;
    }
}
