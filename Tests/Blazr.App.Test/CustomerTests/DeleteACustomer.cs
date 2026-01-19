/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using Blazr.Manganese;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class CustomerTests
{
    [Fact]
    public async Task DeleteACustomer()
    {
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the test item and it's Id from the Test Provider
        var controlItem = _testDataProvider.Customers.Skip(Random.Shared.Next(10)).First();
        var controlRecord = this.AsDmoCustomer(controlItem);
        var controlId = controlRecord.Id;

        var customerAddResult = await mediator.DispatchAsync(CustomerCommandRequest.Create(controlRecord, RecordState.DeletedState));

        Assert.False(customerAddResult.HasException);

        var customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(controlId));

        // check it matches the test record
        Assert.True(customerResult.HasException);
    }
}
