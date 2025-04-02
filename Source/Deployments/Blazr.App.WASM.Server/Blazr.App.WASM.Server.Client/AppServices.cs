/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation;
using Blazr.Auth.Core;
using Blazr.Gallium;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Toast;
using Blazr.App.Weather.API;

namespace Blazr.App.WASM.Server.Client;

public static class ApplicationServerServices
{
    public static void AddAppServices(this IServiceCollection services, string baseHostEnvironmentAddress)
    {
        services.AddScoped<AuthenticationStateProvider, VerySimpleAuthenticationStateProvider>();
        services.AddAppPolicyServices();

        services.AddAuthentication( options => options.DefaultScheme = TestIdentities.Provider)
            .AddScheme<VerySimpleAuthSchemeOptions, VerySimpleAuthenticationHandler>(TestIdentities.Provider, options => { });

        services.AddAuthorizationCore(config =>
        {
            foreach (var policy in AppPolicies.Policies)
            {
                config.AddPolicy(policy.Key, policy.Value);
            }
        });

        services.AddCascadingAuthenticationState();
        services.AddAuthenticationStateDeserialization();

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(Blazr.App.Weather.API.WeatherApplicationServerServices).Assembly
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

        //services.AddInvoiceAppServices();
        services.AddWeatherAppAPIServices( baseHostEnvironmentAddress );
    }
}
