/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public abstract record BaseAction<T>
    where T : BaseAction<T>
{
    public Guid TransactionId { get; protected init; } = default!;
    public object? Sender { get; protected init; } = default!;


    public T AddSender(object? sender)
        => (T)this with { Sender = sender };

    public T AddTransactionId(Guid transactionId)
        => (T)this with { TransactionId = transactionId };
}
