/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.App.Core;
using Blazr.App.Invoice.Core;
using MediatR;

namespace Blazr.App.Invoice.Infrastructure;

/// <summary>
/// Mediatr Handler for executing list requests against a Customer Entity
/// </summary>
public sealed record CustomerLookUpHandler : IRequestHandler<CustomerLookupRequest, Result<ListItemsProvider<CustomerLookUpItem>>>
{
    private IListRequestBroker _broker;

    public CustomerLookUpHandler(IListRequestBroker broker)
    {
        this._broker = broker;
    }

    public async Task<Result<ListItemsProvider<CustomerLookUpItem>>> Handle(CustomerLookupRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<CustomerLookUpItem> records = Enumerable.Empty<CustomerLookUpItem>();

        var query = new ListQueryRequest<CustomerLookUpItem>()
        {
            PageSize = 10000,
            StartIndex = 0,
            SortDescending = true,
            SortExpression = (Item) => Item.Name,
            FilterExpression = null,
            Cancellation = cancellationToken
        };

        var result = await _broker.ExecuteAsync<CustomerLookUpItem>(query);

        if (!result.HasSucceeded(out ListItemsProvider<CustomerLookUpItem>? listResult))
            return result.ConvertFail<ListItemsProvider<CustomerLookUpItem>>();

        return Result<ListItemsProvider<CustomerLookUpItem>>.Success( new(listResult.Items, listResult.TotalCount));
    }
}
