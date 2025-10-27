/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public readonly record struct InvoiceCommandRequest(
        InvoiceEntity Item, EditState State, Guid TransactionId)
    : IRequest<Result>
{
    public static InvoiceCommandRequest Create(InvoiceMutor mutor)
        => new InvoiceCommandRequest(mutor.CurrentEntity, mutor.State, Guid.NewGuid());
}
