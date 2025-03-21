/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.Antimony.Infrastructure.Server;
using Blazr.App.Invoice.Core;
using MediatR;
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

    public async Task<Result<ListItemsProvider<DmoCustomer>>> Handle(CustomerListRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        IEnumerable<DmoCustomer> forecasts = Enumerable.Empty<DmoCustomer>();

        var query = new ListQueryRequest<DboCustomer>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

        var result = await dbContext.GetItemsAsync<DboCustomer>(query);

        if (!result.HasSucceeded(out ListItemsProvider<DboCustomer>? listResult))
            return result.ConvertFail<ListItemsProvider<DmoCustomer>>();

        var list = listResult.Items.Select(item => CustomerMap.Map(item));

        return Result<ListItemsProvider<DmoCustomer>>.Success( new(list, listResult.TotalCount));
    }

    private Expression<Func<DboCustomer, object>> GetSorter(string? field)
        => field switch
        {
            AppDictionary.Customer.CustomerName => (Item) => Item.CustomerName,
            _ => (item) => item.CustomerID
        };

    // No Filter Defined
    private Expression<Func<DboCustomer, bool>>? GetFilter(CustomerListRequest request)
    {
        return null;
    }
}
