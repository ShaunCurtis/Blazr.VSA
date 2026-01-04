/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public readonly record struct InvoiceEntityCommandRequest(
        InvoiceEntity Item, EditState State, Guid TransactionId)
    : IRequest<Return>
{
    public static InvoiceEntityCommandRequest Create(InvoiceEntity entity, EditState state)
        => new InvoiceEntityCommandRequest(entity, state, Guid.NewGuid());
}
