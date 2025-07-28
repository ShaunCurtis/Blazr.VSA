/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Blazr.Gallium;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium.Presentation;

// This is the boilerplate code for any GridUIBroker
// It provides the basic functionality for a Grid Broker

public partial class GridUIBroker<TRecord, TKey>
    : IGridUIBroker<TRecord>, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public Guid StateContextUid { get; private set; } = Guid.NewGuid();
    public GridState<TRecord> GridState { get; private set; } = new();
    public Result LastResult { get; protected set; } = Result.Success();

    public event EventHandler<EventArgs>? StateChanged;

    public GridUIBroker(IMessageBus messageBus, IEntityProvider<TRecord, TKey> entityProvider, ScopedStateProvider scopedStateProvider)
    {
        _messageBus = messageBus;
        _gridStateStore = scopedStateProvider;
        _entityProvider = entityProvider;

        _messageBus.Subscribe<TRecord>(this.OnStateChanged);
    }

    public void Dispose()
        => _messageBus.UnSubscribe<TRecord>(this.OnStateChanged);

    public Result SetContext(Guid context, UpdateGridRequest<TRecord> resetGridRequest)
    {
        this.StateContextUid = context;

        return this.Dispatch(resetGridRequest).ToResult;
    }

    public Result SetContext(Guid context)
    {
        this.StateContextUid = context;

        // Check to see if we have a saved grid state for this context.
        // If so load and apply it
        return _gridStateStore.GetState<GridState<TRecord>>(context)
            .UpdateState(hasValue: (state) => this.GridState = state,
                hasException: (ex) => this.GridState = new GridState<TRecord>())
            .ToResult;
    }

    public Result<GridState<TRecord>> Dispatch(UpdateGridRequest<TRecord> request)
        => Result<GridState<TRecord>>
            .Create(new()
            {
                Key = this.StateContextUid,
                PageSize = request.PageSize,
                StartIndex = request.StartIndex,
                SortField = request.SortField,
                SortDescending = request.SortDescending
            })
            .Dispatch(_gridStateStore.Dispatch);

    public async ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync(GridItemsProviderRequest<TRecord> gridRequest)
        => await Result<UpdateGridRequest<TRecord>>
            .Create(UpdateGridRequest<TRecord>.Create(gridRequest))
            .Dispatch(this.Dispatch)
            .ApplyTransformAsync(_entityProvider.GetItemsAsync)
            .MutateStateAsync((result) => this.LastResult = result)
            .OutputValueAsync(ExceptionOutput: (ex) => GridItemsProviderResult.From<TRecord>(new List<TRecord>(), 0));

    // Services
    protected readonly IMessageBus _messageBus;
    private readonly ScopedStateProvider _gridStateStore;
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;

    private void OnStateChanged(object? message)
        => this.StateChanged?.Invoke(this, EventArgs.Empty);

}

