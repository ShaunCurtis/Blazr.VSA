/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public record StateRecord<T>
{
    public T Record { get; init; }
    public EditState State { get; init; }
    public readonly Guid TransactionId = Guid.CreateVersion7();

    public StateRecord(T record, EditState state)
    {
        this.Record = record;
        this.State = state;
    }

    public StateRecord(T record, EditState state, Guid transactionId)
    {
        this.Record = record;
        this.State = state;
        this.TransactionId = transactionId;
    }

    public bool IsDirty
        => this.State != EditState.Clean;

    public static StateRecord<T> Create(T record, EditState state, Guid? transactionId = null)
        => new(record, state, transactionId ?? Guid.CreateVersion7());
}
