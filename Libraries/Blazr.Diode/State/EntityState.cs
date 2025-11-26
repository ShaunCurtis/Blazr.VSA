/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public sealed class EntityState<T>
    where T : notnull
{
    public EditState State => _currentState.State;
    public T Record => _currentState.Record;

    public bool IsDirty
        => _currentState.State != EditState.Clean;

    public StateRecord<T> AsStateRecord
        => _currentState;

    public EntityState(T item, bool isNew = false)
    {
        _currentState = StateRecord<T>.Create(item, isNew
            ? EditState.New
            : EditState.Clean);
        _baseState = _currentState;
    }

    public Bool Reset()
    {
        _currentState = _baseState;
        _lastState = null;
        return Bool.Success();
    }

    public Bool Update(T record, Guid transactionId)
        => this.SaveState(transactionId)
            .Bind(() => UpdateState(record));

    public Bool MarkAsDeleted(Guid transactionId)
        => this.SaveState(transactionId)
            .Bind(DeleteState);

    public Bool MarkAsPersisted()
            => this.PersistedState();
    public Bool RollBackLastUpdate(Guid transactionId)
        => RollBackState(transactionId);

    //==============================================================

    private StateRecord<T>? _lastState;
    private StateRecord<T> _currentState;
    private StateRecord<T> _baseState;

    private Bool PersistedState()
    {
        _currentState = _currentState with { State = EditState.Clean };
        return Bool.Success();
    }

    private Bool DeleteState()
    {
        _currentState = _currentState with { State = EditState.Deleted };
        return Bool.Success();
    }

    private Bool SaveState(Guid transactionId)
    {
        _lastState = this.AsStateRecord with { TransactionId = transactionId };
        return Bool.Success();
    }

    private Bool UpdateState(T record)
    {
        if (!this.Record.Equals(record))
            _currentState = new StateRecord<T>(record, this.State.AsDirty);
        return Bool.Success();
    }

    private Bool RollBackState(Guid transactionId)
    {
        if (_lastState is null || _lastState.TransactionId != transactionId)
            Bool.Failure("There is no rollback data for the transaction.");

        _currentState = _lastState!;
        _lastState = null;

        return Bool.Success();
    }
}

