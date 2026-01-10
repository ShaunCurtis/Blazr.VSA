/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public readonly record struct InvoiceEntityRequest(InvoiceId Id)
    : IRequest<Result<InvoiceEntity>>
{
    public static InvoiceEntityRequest Create(InvoiceId id)
        => new InvoiceEntityRequest(id);
}

