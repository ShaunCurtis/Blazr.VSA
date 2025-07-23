/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.Cadmium.Presentation;

public partial class EditUIBroker<TRecord, TRecordEditContext, TKey> : IEditUIBroker<TRecordEditContext, TKey>
    where TRecord : class, new()
    where TRecordEditContext : IRecordEditContext<TRecord>, new()
    where TKey : notnull, IEntityId
{
    public EditState State { get; protected set; } = EditState.Clean;

    public Result LastResult { get; protected set; } = Result.Success();

    public TRecordEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public EditUIBroker(IEntityProvider<TRecord, TKey> entityProvider)
    {
        _entityProvider = entityProvider;
        this.EditContext = new EditContext(EditMutator);
    }

    public async ValueTask<Result> LoadAsync(TKey recordId)
    {
        this.LastResult = await LoadRecordAsync(recordId);
        return this.LastResult;
    }

    public ValueTask<Result> ResetAsync()
    {
        this.LastResult = ResetItem();
        return LastResult.CompletedValueTask;
    }

    public async ValueTask<Result> SaveAsync(bool refreshOnNew = true)
    {
        this.LastResult = await this.UpdateRecordAsync(refreshOnNew);
        return this.LastResult;
    }

    public async ValueTask<Result> DeleteAsync()
    {
        this.LastResult = await DeleteItemAsync();
        return this.LastResult;
    }
}

public partial class EditUIBroker<TRecord, TRecordEditContext, TKey> : IEditUIBroker<TRecordEditContext, TKey>
    where TRecord : class, new()
    where TRecordEditContext : IRecordEditContext<TRecord>, new()
    where TKey : notnull, IEntityId
{
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;

    private bool _isLoaded;

    private async Task<Result> LoadRecordAsync(TKey recordId)
        => await Result<TKey>.Create(recordId)
            // Set the broker state
            .ApplySideEffect(
                test: recordId.IsDefault,
                isTrue: id => this.State = EditState.New,
                isFalse: id => this.State = EditState.Clean)
            // Check if the broker has already been loaded
            .ApplyTransform<TKey>(
                test: _isLoaded,
                trueTransform: id => Result<TKey>.Failure("The UIBroker has already been loaded."),
                falseTransform: id => Result<TKey>.Create(id))
            // Get the record item.  This will return a new record if the id is default
            .ApplyTransformAsync<TRecord>(_entityProvider.RecordRequestAsync)
            // Set up the EditMutator and EditContext
            .TaskSideEffectAsync<TRecord>(
                success: record =>
                {
                    this.EditMutator = new();
                    this.EditMutator.Load(record!);
                    this.EditContext = new EditContext(EditMutator);
                    _isLoaded = true;
                })
            .MapTaskToResultAsync();

    private async Task<Result> UpdateRecordAsync()
        => await UpdateRecordAsync(refreshOnNew: true);

    private async Task<Result> UpdateRecordAsync(bool refreshOnNew)
        => await Result.Success()
            // Check we're loaded
            .ApplyTransformOnException(!_isLoaded, "No record is loaded.")
            // Get the record item from the EditMutator
            .ApplyTransform<TRecord>(() => EditMutator.ToResult)
             // Set the broker state to dirty
             .ApplySideEffect(hasValue: (value) => this.State = this.State.AsDirty)
             // Save the record item to the datastore
             .ApplyTransformAsync<TKey>((record) => _entityProvider.RecordCommandAsync(StateRecord<TRecord>.Create(record, this.State)))
             // Set the broker state to not loaded
             .TaskSideEffectAsync(test: refreshOnNew, isTrue: (id) => _isLoaded = false)
             // Refresh the record if refreshOnNew is set
             .MapTaskToResultAsync(test: refreshOnNew, isTrue: LoadRecordAsync);

    private Result ResetItem()
        => Result.Success()
            // Check we're loaded
            .ApplyTransformOnException(!_isLoaded, "No record is loaded.")
            // Set the broker state
            .ApplySideEffect(() =>
                {
                    EditMutator.Reset();
                    // Create a new EditContext - will reset and rebuild the whole Edit Form
                    this.EditContext = new EditContext(EditMutator);
                });

    private async ValueTask<Result> DeleteItemAsync()
        => await Result.Success()
            // Check we're loaded
            .ApplyTransformOnException(!_isLoaded, "No record is loaded.")
            // Set the broker state
            .ApplySideEffect(() => this.State = EditState.Deleted)
            // Delete the record item from the datastore
            .ApplyTransformAsync(this.UpdateRecordAsync);
}
