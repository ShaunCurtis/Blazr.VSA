/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.InvoiceRequests;

namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing list requests against a Invoice Entity
/// </summary>
public sealed class InvoiceListHandler : IRequestHandler<InvoiceListRequest, Result<ListResult<DmoInvoice>>>
{
    private IListRequestBroker _broker;

    public InvoiceListHandler(IListRequestBroker broker)
    {
        this._broker = broker;
    }

    public async Task<Result<ListResult<DmoInvoice>>> Handle(InvoiceListRequest request, CancellationToken cancellationToken)
    {
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

        var result = await _broker.ExecuteAsync<DvoInvoice>(query);

        if (!result.HasSucceeded(out ListResult<DvoInvoice> listResult))
            return result.ConvertFail<ListResult<DmoInvoice>>();

        var list = listResult.Items.Select(item => DvoInvoiceMap.Map(item));

        return Result<ListResult<DmoInvoice>>.Success( new(list, listResult.TotalCount));
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
