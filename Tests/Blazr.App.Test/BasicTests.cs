using Blazr.App.Core;
using Blazr.Diode.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazr.App.Test;

public partial class BasicTests
{
    [Fact]
    public async Task GetACustomer()
    {
        var customer = DmoCustomer.NewCustomer();

        var test = customer with { };

        Assert.Equivalent(customer, test);
    }
}
