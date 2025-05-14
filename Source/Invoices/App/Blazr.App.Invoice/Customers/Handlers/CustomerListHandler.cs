/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.Antimony.Mediator;
using Blazr.Antimony.Infrastructure.EntityFramework;
using Blazr.App.Invoice.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing list requests against a Customer Entity
/// </summary>
public sealed class CustomerListHandler : IRequestHandler<CustomerListRequest, Result<ListItemsProvider<DmoCustomer>>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerListHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
    {
        _factory = factory;
    }
    public async Task<Result<ListItemsProvider<DmoCustomer>>> HandleAsync(CustomerListRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        IEnumerable<DmoCustomer> forecasts = Enumerable.Empty<DmoCustomer>();

        var query = new ListQueryRequest<DvoCustomer>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

        var result = await dbContext.GetItemsAsync<DvoCustomer>(query);

        if (!result.HasSucceeded(out ListItemsProvider<DvoCustomer>? listResult))
            return result.ConvertFail<ListItemsProvider<DmoCustomer>>();

        var list = listResult.Items.Select(item => item.ToDmo());

        return Result<ListItemsProvider<DmoCustomer>>.Success( new(list, listResult.TotalCount));
    }

    private Expression<Func<DvoCustomer, object>> GetSorter(string? field)
        => field switch
        {
            AppDictionary.Customer.CustomerName => (Item) => Item.CustomerName,
            _ => (item) => item.CustomerID
        };

    // No Filter Defined
    private Expression<Func<DvoCustomer, bool>>? GetFilter(CustomerListRequest request)
    {
        return null;
    }
}
