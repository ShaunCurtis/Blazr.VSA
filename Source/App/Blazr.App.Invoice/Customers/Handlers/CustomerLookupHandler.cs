/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.Antimony.Infrastructure.EntityFramework;
using Blazr.App.Invoice.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Invoice.Infrastructure;

/// <summary>
/// Mediatr Handler for executing list requests against a Customer Entity
/// </summary>
public sealed record CustomerLookUpHandler : IRequestHandler<CustomerLookupRequest, Result<ListItemsProvider<CustomerLookUpItem>>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerLookUpHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<ListItemsProvider<CustomerLookUpItem>>> Handle(CustomerLookupRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

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

        var result = await dbContext.GetItemsAsync<CustomerLookUpItem>(query);

        if (!result.HasSucceeded(out ListItemsProvider<CustomerLookUpItem>? listResult))
            return result.ConvertFail<ListItemsProvider<CustomerLookUpItem>>();

        return Result<ListItemsProvider<CustomerLookUpItem>>.Success(new(listResult.Items, listResult.TotalCount));
    }
}
