/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation.Bootstrap;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Infrastructure;

public static class CustomerServices
{
    public static void AddCustomerServices(this IServiceCollection services)
    {
        services.AddScoped<IEntityProvider<DmoCustomer, CustomerId>, CustomerEntityProvider>();
        services.AddSingleton<IUIEntityProvider<DmoCustomer>, CustomerUIEntityProvider>();

        services.AddTransient<IGridPresenter<DmoCustomer>, CustomerGridPresenter>();
        services.AddTransient<IEditPresenter<CustomerEditContext, CustomerId>, EditPresenter<DmoCustomer, CustomerEditContext, CustomerId>>();
        services.AddTransient<IReadPresenter<DmoCustomer, CustomerId>, ReadPresenter<DmoCustomer,CustomerId>>();
    }
}
