/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public readonly record struct CustomerCommandRequest(
        StateRecord<DmoCustomer> Item)
    : IRequest<Result<CustomerId>>
{
    public static CustomerCommandRequest Create(DmoCustomer item, EditState state)
        => new CustomerCommandRequest(new(item, state));

    public static CustomerCommandRequest Create(DmoCustomer item, EditState state, Guid transactionId)
        => new CustomerCommandRequest(new(item, state, transactionId));
}
