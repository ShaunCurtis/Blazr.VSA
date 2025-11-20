/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public sealed record FKProvider
{
    private readonly IMediatorBroker _mediatorBroker;

    public FKProvider(IMediatorBroker mediatorBroker)
        => _mediatorBroker = mediatorBroker;

    public Task<Bool<IEnumerable<FkoCustomer>>> GetCustomerFKAsync()
        => _mediatorBroker.DispatchAsync(new CustomerFKRequest());
}
