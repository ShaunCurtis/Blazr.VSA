/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.EntityFramework;

public static partial class AppBlazorServerServices
{
    public static void AddAppBlazorServerServices(this IServiceCollection services)
    {
        // Add any individual entity services
        services.AddCustomerServices();
        //services.AddInvoiceServices();
    }
}
