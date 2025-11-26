/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing list requests against a Customer Entity in a Entity Framework Context
/// </summary>
public sealed class InvoiceListHandler : IRequestHandler<InvoiceListRequest, Bool<ListItemsProvider<DmoInvoice>>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public InvoiceListHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
        => _factory = factory;

    public async Task<Bool<ListItemsProvider<DmoInvoice>>> HandleAsync(InvoiceListRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        var query = new ListQueryRequest<DvoInvoice>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

        return await dbContext.GetItemsAsync<DvoInvoice>(query)
           .BindAsync((provider) =>
                Bool<ListItemsProvider<DmoInvoice>>
                    .Read(new ListItemsProvider<DmoInvoice>(
                        Items: provider.Items.Select(item => DvoInvoice.Map(item)),
                        TotalCount: provider.TotalCount))
            );

    }

    private Expression<Func<DvoInvoice, object>> GetSorter(string? field)
        => field switch
        {
            "Id" => (Item) => Item.InvoiceID,
            "ID" => (Item) => Item.InvoiceID,
            "Name" => (Item) => Item.CustomerName ?? "No Customer Name Set",
            _ => (item) => item.InvoiceID // Default Sort by ID
        };

    // No Filter Defined
    private Expression<Func<DvoInvoice, bool>>? GetFilter(InvoiceListRequest request)
    {
        return null;
    }
}
