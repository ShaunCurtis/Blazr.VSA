/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using MediatR;


namespace Blazr.App.Invoice.Core;

public readonly record struct CustomerCommandRequest(
        DmoCustomer Item,
        CommandState State)
    : IRequest<Result<CustomerId>>;
