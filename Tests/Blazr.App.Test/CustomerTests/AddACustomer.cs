/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class CustomerTests
{
    [Fact]
    public async Task AddACustomer()
    {
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        var newCustomer = DmoCustomer.NewCustomer() with { Name = new("Alaskan") };

        var customerAddResult = await mediator.DispatchAsync(CustomerCommandRequest.Create(newCustomer, EditState.New));

        Assert.False(customerAddResult.HasException);

        var customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(newCustomer.Id));

        // check it matches the test record

        // NewCustomer has the isNew flag set in the record so we need to fix that to make a compare
        var customer = newCustomer with { Id = new(newCustomer.Id.Value) };
        Assert.False(customerResult.HasException);
        Assert.Equivalent(customer, customerResult.Value);
    }
}
