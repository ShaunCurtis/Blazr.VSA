/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Core.Invoices;
using Blazr.Diode.Mediator;
using Blazr.Manganese;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class InvoiceTests
{
    [Theory]
    [InlineData(0, 2)]
    [InlineData(0, 20)]
    [InlineData(1, 2)]
    public async Task GetInvoiceGrid(int startIndex, int pageSize)
    {
        var provider = GetServiceProvider();

        // Get the total expected count and the first record of the page
        var testCount = _testDataProvider.Invoices.Count();
        var testPageCount = _testDataProvider.Invoices.Skip(startIndex).Take(pageSize).Count();

        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        var request = new InvoiceListRequest()
        {
            PageSize = pageSize,
            StartIndex = startIndex,
            SortColumn = null,
            SortDescending = false,
        };

        var listResult = await mediator.DispatchAsync(request);

        Assert.True(listResult.HasValue);
        Assert.Equal(testCount, listResult.AsSuccess.Value.TotalCount);
        Assert.Equal(testPageCount, listResult.AsSuccess.Value.Items.Count());
    }
}
