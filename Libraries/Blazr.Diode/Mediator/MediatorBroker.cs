/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode.Mediator;

public class MediatorBroker : IMediatorBroker
{
    private readonly IServiceProvider _serviceProvider;
    
    public MediatorBroker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        // Get the handler tyoe based on the request
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        // Get the handler from DI
        dynamic? handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
            throw new InvalidOperationException($"Handler for {request.GetType().Name} not found.");

        var result = await handler.HandleAsync((dynamic)request, cancellationToken);

        return result;
    }
}
