/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public readonly record struct CustomerCommandRequest(
    DmoCustomer Item, RecordState State)
    : IRequest<Return<CustomerId>>
{
    public static CustomerCommandRequest Create(DmoCustomer item, RecordState state)
        => new CustomerCommandRequest(item, state);

    public static CustomerCommandRequest Create(CustomerRecordMutor mutor)
        => new CustomerCommandRequest(mutor.Record, mutor.State);
}
