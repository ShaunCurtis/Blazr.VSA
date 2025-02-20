/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;

namespace Blazr.App.Presentation;

public class EditPresenter<TRecord, TRecordEditContext, TKey> : IEditPresenter<TRecordEditContext, TKey>
    where TRecord : class, new()
    where TRecordEditContext : IRecordEditContext<TRecord>, new()
    where TKey : notnull, IEntityId
{
    protected readonly IMediator Databroker;
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;

    protected TKey EntityId = default!;
    private bool _isLoaded;
    public CommandState CommandState { get; private set; } = CommandState.None;

    public IDataResult LastResult { get; protected set; } = DataResult.Success();

    public TRecordEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public EditPresenter(IMediator mediator, IEntityProvider<TRecord, TKey> entityProvider)
    {
        this.Databroker = mediator;
        _entityProvider = entityProvider;

        this.EditContext = new EditContext(EditMutator);
    }

    public async ValueTask LoadAsync(TKey? id)
    {
        if (_isLoaded)
        {
            LastResult = DataResult.Failure("The Presenter has already been loaded. You cannot reload the Presenter.");
            return;
        }

        // check if we have a real Id to get
        if (id is TKey key && !key.IsDefault)
        {
            this.EntityId = key;
            await GetRecordItemAsync();
            return;
        }

        // We don't have a real Id, so we need to initialize with a new item
        await this.GetNewItemAsync();
    }

    public ValueTask ResetItemAsync()
    {
        Debug.Assert(_isLoaded);

        EditMutator.Reset();

        // Create a new EditContext.
        // This will reset and rebuild the whole Edit Form
        this.EditContext = new EditContext(EditMutator);

        return ValueTask.CompletedTask;
    }

    public ValueTask SaveItemAsync(bool refreshOnNew = true)
    {
        Debug.Assert(_isLoaded);

        return this.UpdateRecordAsync(refreshOnNew);
    }

    public async ValueTask DeleteItemAsync()
    {
        Debug.Assert(_isLoaded);

        this.CommandState = CommandState.Delete;
        await this.UpdateRecordAsync();
    }

    private ValueTask GetNewItemAsync()
    {
        this.LastResult = DataResult.Success();

        var record = _entityProvider.NewRecord;

        this.EditMutator = new();
        this.EditMutator.Load(record);

        this.EditContext = new EditContext(EditMutator);

        this.CommandState = CommandState.Add;
        _isLoaded = true;

        return ValueTask.CompletedTask;
    }

    private async ValueTask GetRecordItemAsync()
    {
        this.LastResult = DataResult.Success();

        var result = await _entityProvider.RecordRequest.Invoke(Databroker, this.EntityId);

        if (!result.HasSucceeded(out TRecord? record))
        {
            this.LastResult = result.ToDataResult;
            _isLoaded = true;
            return;
        }

        this.EditMutator = new();
        this.EditMutator.Load(record!);

        this.EditContext = new EditContext(EditMutator);

        _isLoaded = true;
    }

    private async ValueTask UpdateRecordAsync(bool refreshOnNew = true)
    {
        LastResult = DataResult.Failure("Nothing to Do");

        // Update the command state for an update operation
        if (this.CommandState == CommandState.None)
            this.CommandState = this.EditMutator.IsDirty ? CommandState.Update : this.CommandState;

        var mutatedResult = EditMutator.AsRecord;


        var commandResult = await _entityProvider.RecordCommand.Invoke(this.Databroker, mutatedResult, this.CommandState);

        this.LastResult = commandResult.ToDataResult;

        if (!commandResult.HasSucceeded(out TKey? key))
            return;

        if (this.CommandState == CommandState.Add && refreshOnNew)
        {
            this.EntityId = _entityProvider.GetKey(key);
            await GetRecordItemAsync();
        }
    }
}