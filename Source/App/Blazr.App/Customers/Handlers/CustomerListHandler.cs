/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing list requests against a Customer Entity
/// </summary>
public sealed class CustomerListHandler : IRequestHandler<CustomerListRequest, Result<ListResult<DmoCustomer>>>
{
    private IListRequestBroker _broker;

    public CustomerListHandler(IListRequestBroker broker)
    {
        this._broker = broker;
    }

    public async Task<Result<ListResult<DmoCustomer>>> Handle(CustomerListRequest request, CancellationToken cancellationToken)
    {
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

        var result = await _broker.ExecuteAsync<DboCustomer>(query);

        if (!result.HasSucceeded(out ListResult<DboCustomer> listResult))
            return result.ConvertFail<ListResult<DmoCustomer>>();

        var list = listResult.Items.Select(item => DboCustomerMap.Map(item));

        return Result<ListResult<DmoCustomer>>.Success( new(list, listResult.TotalCount));
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
