/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class CustomerTests
{
    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 20)]
    [InlineData(5, 10)]
    public async Task GetCustomerGrid(int startIndex, int pageSize)
    {
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the total expected count and the first record of the page
        var testCount = _testDataProvider.Customers.Count();
        var testFirstItem = _testDataProvider.Customers.Skip(startIndex).First();
        var testFirstRecord = this.AsDmoCustomer(testFirstItem);

        var customerListResult = await mediator.DispatchAsync(new CustomerListRequest()
        {
            PageSize = pageSize,
            StartIndex = startIndex,
            SortColumn = null,
            SortDescending = false
        });

        Assert.False(customerListResult.HasException);
        Assert.Equal(testCount, customerListResult.Value!.TotalCount);
        Assert.Equal(pageSize, customerListResult.Value!.Items.Count());
        //Assert.Equal(testFirstRecord, customerListResult.Value!.Items.First());
    }
}
