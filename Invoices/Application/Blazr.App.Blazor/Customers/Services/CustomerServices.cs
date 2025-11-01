/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App;

public static class CustomerServices
{
    public static void AddCustomerServices(this IServiceCollection services)
    {
        services.AddScoped<IUIConnector<DmoCustomer, CustomerId>, CustomerUIConnector>();
    }
}
