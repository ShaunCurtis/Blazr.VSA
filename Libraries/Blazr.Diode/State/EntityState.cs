/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public sealed class EntityState<T>
{
    private StateRecord<T>? _lastState;

    public EditState State { get; private set; }
        = EditState.Clean;

    public T Record { get; private set; }

    public bool IsDirty
        => this.State != EditState.Clean;

    public StateRecord<T> AsRecord
        => new(this.Record, this.State);

    public EntityState(T item, bool isNew = false)
    {
        this.Record = item;
        this.State = isNew
            ? EditState.New
            : EditState.Clean;
    }

    public Result Reset(StateRecord<T> stateRecord)
    {
        this.Record = stateRecord.Record;
        this.State = stateRecord.State;
        _lastState = null;
        return Result.Success();
    }

    public Result Update(T record, Guid transactionId)
    {
        this.SaveState(transactionId);

        this.Record = record;
        this.State = this.State.AsDirty;
        return Result.Success();
    }

    public Result MarkAsDeleted(Guid transactionId)
    {
        this.SaveState(transactionId);

        this.State = EditState.Deleted;
        return Result.Success();
    }

    public Result MarkAsPersisted()
    {
        this.State = EditState.Clean;
        return Result.Success();
    }

    private void SaveState(Guid? transactionId = null)
        => _lastState = new(this.Record, this.State, transactionId ?? Guid.NewGuid());

    public Result RollBackLastUpdate(Guid transactionId)
    {
        if (this._lastState is null)
            return Result.Failure("No rollback state available.");

        if (_lastState?.TransactionId != transactionId)
            return Result.Failure("There is no rollback data for the transaction.");


        this.Record = _lastState.Record;
        this.State = _lastState.State;
        _lastState = null;

        return Result.Success();
    }
}

