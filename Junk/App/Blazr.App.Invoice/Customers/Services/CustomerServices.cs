/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.App.Invoice.Core;
using Blazr.App.Invoice.UI;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Invoice.Infrastructure;

public static class CustomerServices
{
    public static void AddCustomerServices(this IServiceCollection services)
    {
        services.AddScoped<IEntityProvider<DmoCustomer, CustomerId>, CustomerEntityProvider>();
        services.AddSingleton<IUIEntityProvider<DmoCustomer>, CustomerUIEntityProvider>();

        services.AddTransient<IGridUIBroker<DmoCustomer>, GridUIBroker<DmoCustomer,CustomerId>>();
        services.AddTransient<IEditUIBroker<CustomerEditContext, CustomerId>, EditUIBroker<DmoCustomer, CustomerEditContext, CustomerId>>();
        services.AddTransient<IReadUIBroker<DmoCustomer, CustomerId>, ReadUIBroker<DmoCustomer,CustomerId>>();
    }
}
