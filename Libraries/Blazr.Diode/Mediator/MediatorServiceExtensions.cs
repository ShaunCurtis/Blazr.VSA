/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Blazr.Diode.Mediator;

public static class Mediator
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddScoped<IMediatorBroker, MediatorBroker>();

        foreach (var assembly in assemblies)
            AddAssemblyHandlers(services, assembly);

        return services;
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        services.AddScoped<IMediatorBroker, MediatorBroker>();

        AddAssemblyHandlers(services, assembly);

        return services;
    }

    private static void AddAssemblyHandlers(IServiceCollection services, Assembly assembly)
    {
        var requestHandlerInterfaceType = typeof(IRequestHandler<,>);

        var handlerTypes = assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type => type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == requestHandlerInterfaceType)
                .Select(i => new { Interface = i, Implementation = type }));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
    }
}
