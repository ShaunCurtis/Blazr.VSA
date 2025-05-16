/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Antimony.Mediator;


public interface IRequestHandler<in TRequest, TResponse> 
    where TRequest : IRequest<TResponse> 
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
