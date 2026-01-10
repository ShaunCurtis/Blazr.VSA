/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public readonly record struct CustomerRecordRequest(CustomerId Id) 
    : IRequest<Result<DmoCustomer>>
{
    public static CustomerRecordRequest Create(CustomerId Id)
        => new(Id);
}
