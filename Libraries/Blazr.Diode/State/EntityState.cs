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

    public Result Reset()
    {
        _currentState = _baseState;
        _lastState = null;
        return Result.Success();
    }

    public Result Update(T record, Guid transactionId)
        => this.SaveState(transactionId)
            .ExecuteActionWithResult(() => UpdateState(record));

    public Result MarkAsDeleted(Guid transactionId)
        => this.SaveState(transactionId)
            .ExecuteActionWithResult(DeleteState);

    public Result MarkAsPersisted()
            => this.PersistedState();
    public Result RollBackLastUpdate(Guid transactionId)
        => RollBackState(transactionId);

    //==============================================================

    private StateRecord<T>? _lastState;
    private StateRecord<T> _currentState;
    private StateRecord<T> _baseState;

    private Result PersistedState()
    {
        _currentState = _currentState with { State = EditState.Clean };
        return Result.Success();
    }

    private Result DeleteState()
    {
        _currentState = _currentState with { State = EditState.Deleted };
        return Result.Success();
    }

    private Result SaveState(Guid transactionId)
    {
        _lastState = this.AsStateRecord with { TransactionId = transactionId };
        return Result.Success();
    }

    private Result UpdateState(T record)
    {
        if (!this.Record.Equals(record))
            _currentState = new StateRecord<T>(record, this.State.AsDirty);
        return Result.Success();
    }

    private Result RollBackState(Guid transactionId)
        => Result<StateRecord<T>>.Create(_lastState)
        .ExecuteConditionalTransaction(
            conditionalTest: (state) => state.TransactionId != transactionId,
            function: (state) => Result<StateRecord<T>>.Failure("There is no rollback data for the transaction.")
            )
            .ExecuteAction((state) =>
            {
                _currentState = state;
                _lastState = null;
            })
            .ToResult();
}

