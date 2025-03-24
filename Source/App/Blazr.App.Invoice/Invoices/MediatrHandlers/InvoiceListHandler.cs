/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.Antimony.Infrastructure.EntityFramework;
using Blazr.App.Invoice.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Blazr.App.Invoice.Core.InvoiceRequests;


namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing list requests against a Invoice Entity
/// </summary>
public sealed class InvoiceListHandler : IRequestHandler<InvoiceListRequest, Result<ListItemsProvider<DmoInvoice>>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public InvoiceListHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> _factory)
    {
        this._factory = _factory;
    }

    public async Task<Result<ListItemsProvider<DmoInvoice>>> Handle(InvoiceListRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        IEnumerable<DmoInvoice> forecasts = Enumerable.Empty<DmoInvoice>();

        var query = new ListQueryRequest<DvoInvoice>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

        var result = await dbContext.GetItemsAsync<DvoInvoice>(query);

        if (!result.HasSucceeded(out ListItemsProvider<DvoInvoice>? listResult))
            return result.ConvertFail<ListItemsProvider<DmoInvoice>>();

        var list = listResult.Items.Select(item => InvoiceMap.Map(item));

        return Result<ListItemsProvider<DmoInvoice>>.Success(new(list, listResult.TotalCount));
    }

    private Expression<Func<DvoInvoice, object>> GetSorter(string? field)
        => field switch
        {
            AppDictionary.Invoice.InvoiceID => (Item) => Item.InvoiceID,
            AppDictionary.Invoice.Date => (Item) => Item.Date,
            AppDictionary.Invoice.TotalAmount => (Item) => Item.TotalAmount,
            AppDictionary.Customer.CustomerName => (Item) => Item.CustomerName,
            _ => (item) => item.Date
        };

    // No Filters Defined
    private Expression<Func<DvoInvoice, bool>>? GetFilter(InvoiceListRequest request)
    {
        return null;
    }
}
