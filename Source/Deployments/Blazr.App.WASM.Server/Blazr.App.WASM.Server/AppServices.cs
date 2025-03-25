/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Invoice.Core;
using Blazr.App.Presentation;
using Blazr.Auth.Core;
using Blazr.Gallium;
using Microsoft.AspNetCore.Components.Authorization;
using Blazr.App.Weather.EntityFramework;
using Blazored.Toast;

namespace Blazr.App.WASM.Server;

public static class ApplicationServerServices
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, VerySimpleAuthenticationStateProvider>();
        services.AddAppPolicyServices();

        services.AddAuthentication( options => options.DefaultScheme = TestIdentities.Provider)
            .AddScheme<VerySimpleAuthSchemeOptions, VerySimpleAuthenticationHandler>(TestIdentities.Provider, options => { });

        services.AddAuthorization(config =>
        {
            foreach (var policy in AppPolicies.Policies)
            {
                config.AddPolicy(policy.Key, policy.Value);
            }
        });

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(DmoCustomer).Assembly
                , typeof(Blazr.App.Weather.EntityFramework.WeatherApplicationServerServices).Assembly
                ));

        // Add the Gallium Message Bus Server services
        services.AddScoped<IMessageBus, MessageBus>();

        // Add the Blazored Toast services
        services.AddBlazoredToast();

        // InMemory Scoped State Store 
        services.AddScoped<ScopedStateProvider>();

        // Presenter Factories
        services.AddScoped<ILookupUIBrokerFactory, LookupUIBrokerFactory>();
        services.AddScoped<IEditUIBrokerFactory, EditUIBrokerFactory>();
        services.AddTransient<IReadUIBrokerFactory, ReadUIBrokerFactory>();

        // Add the QuickGrid Entity Framework Adapter
        services.AddQuickGridEntityFrameworkAdapter();

        //services.AddInvoiceAppServices();
        services.AddWeatherAppEFServices();
    }
}
