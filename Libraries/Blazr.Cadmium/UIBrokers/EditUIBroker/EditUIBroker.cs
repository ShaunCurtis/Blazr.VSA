/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.Cadmium.Presentation;

public partial class EditUIBroker<TRecord, TRecordMutor, TKey> : IEditUIBroker<TRecord, TRecordMutor, TKey>
    where TRecordMutor : IRecordMutor<TRecord>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;
    private bool _isLoaded;

    public EditState State { get; protected set; } = EditState.Clean;

    public Result LastResult { get; protected set; } = Result.Success();

    public TRecordMutor EditMutator { get; protected set; }

    public EditContext EditContext { get; protected set; }

    public EditUIBroker(IEntityProvider<TRecord, TKey> entityProvider)
    {
        _entityProvider = entityProvider;
        this.EditMutator = (TRecordMutor)entityProvider.GetNewRecordMutor();
        this.EditContext = new(this.EditMutator);
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
    private async Task<Result> LoadRecordAsync(TKey recordId)
        => await Result<TKey>.Create(recordId)
        .ExecuteTransaction(id =>
        {
            if (_isLoaded)
                Result<TKey>.Failure("The UIBroker has already been loaded.");

            this.State = recordId.IsDefault
                ? EditState.New
                : EditState.Clean;

            return Result<TKey>.Create(id);
        })
        .ExecuteTransformAsync(_entityProvider.RecordRequestAsync)
        .ExecuteActionAsync(record =>
        {
            this.EditMutator = (TRecordMutor)_entityProvider.GetRecordMutor(record);
            this.EditContext = new EditContext(EditMutator);
            _isLoaded = true;
        })
        .ToResultAsync();

    private async Task<Result> UpdateRecordAsync()
        => await UpdateRecordAsync(refreshOnNew: true);

    private async Task<Result> UpdateRecordAsync(bool refreshOnNew)
    => await Result.Success()
        .ExecuteTransform(() =>
        {
            // Check we're loaded
            if (_isLoaded)
                return Result<TRecord>.Failure("No record is loaded.");

            // Set the broker state to dirty
            this.State = State.AsDirty;

            return EditMutator.ToResult();
        })
        // Save the record item to the datastore
        .ExecuteTransformAsync<TKey>(record => _entityProvider.RecordCommandAsync(StateRecord<TRecord>.Create(record, this.State)))
        // Refresh the record if refreshOnNew is set
        .ExecuteTransformOnTrueAsync(
            test: refreshOnNew, 
            truefunction: id =>
            {
                _isLoaded = false;
                return LoadRecordAsync(id);
            }
        );

    private Result ResetItem()
        => Result.Success()
            // Check we're loaded
            .SwitchToException(!_isLoaded, "No record is loaded.")
            // Set the broker state
            .ExecuteAction(() =>
                {
                    EditMutator.Reset();
                    // Create a new EditContext - will reset and rebuild the whole Edit Form
                    this.EditContext = new EditContext(EditMutator);
                });

    private async ValueTask<Result> DeleteItemAsync()
        => await Result.Success()
            // Check we're loaded
            .SwitchToException(!_isLoaded, "No record is loaded.")
            // Set the broker state
            .ExecuteAction(() => this.State = EditState.Deleted)
            // Delete the record item from the datastore
            .ExecuteFunctionAsync(this.UpdateRecordAsync);
}
