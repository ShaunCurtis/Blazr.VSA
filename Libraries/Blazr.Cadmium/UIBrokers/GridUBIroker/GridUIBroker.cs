/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Blazr.Manganese;
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

    /// <summary>
    /// Sets the State context for the Broker 
    /// </summary>
    /// <param name="context"></param>
    public Result SetContext(Guid context, UpdateGridRequest<TRecord> resetGridRequest)
    {
        this.StateContextUid = context;

        return this.DispatchGridStateChange(resetGridRequest)
            .ToResult;
    }

    public Result SetContext(Guid context)
    {
        this.StateContextUid = context;

        // Check to see if we have a saved grid state for this context.
        // If so load and apply it
        return _gridStateStore.GetState<GridState<TRecord>>(context)
            .ApplySideEffect( hasValue: (state) => this.GridState = state,
                hasException: (ex) => this.GridState = new GridState<TRecord>())
            .ToResult;
    }

    /// <summary>
    /// Applies a GridState change to the Broker and saves the state
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Result<GridState<TRecord>> DispatchGridStateChange(UpdateGridRequest<TRecord> request)
        => Result<GridState<TRecord>>
            .Create(new ()
                { 
                    Key = this.StateContextUid,
                    PageSize = request.PageSize,
                    StartIndex = request.StartIndex,
                    SortField = request.SortField,
                    SortDescending = request.SortDescending
                })
            .Dispatch(_gridStateStore.Dispatch);

    public Task<GridItemsProviderResult<TRecord>> GetItemsAsync(GridItemsProviderRequest<TRecord> gridRequest)
        => Result<UpdateGridRequest<TRecord>>
            .Create(UpdateGridRequest<TRecord>.Create(gridRequest))
            .Dispatch(this.DispatchGridStateChange)
            .ApplyTransformAsync(_entityProvider.GetItemsAsync)
            .ApplySideEffectAsync((result) => this.LastResult = result)
            .OutputAsync(ExceptionOutput: (ex) => GridItemsProviderResult.From<TRecord>(new List<TRecord>(), 0));

    public void Dispose()
    {
        _messageBus.UnSubscribe<TRecord>(this.OnStateChanged);
    }
}

public partial class GridUIBroker<TRecord, TKey>
    : IGridUIBroker<TRecord>, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    // Services
    protected readonly IMessageBus _messageBus;
    private readonly ScopedStateProvider _gridStateStore;
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;

    private void OnStateChanged(object? message)
    {
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
