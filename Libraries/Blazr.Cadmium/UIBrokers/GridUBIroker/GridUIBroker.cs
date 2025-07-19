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
    /// Sets the State context for the Broker and retrieves any saved GridState
    /// </summary>
    /// <param name="context"></param>
    public void SetContext(Guid context)
    {
        this.StateContextUid = context;

        // Check to see if we have a saved grid state for this context.
        // If so load and apply it
        if (_gridStateStore.TryGetState<GridState<TRecord>>(context, out GridState<TRecord>? state))
            this.GridState = state;
        else
            this.GridState = new GridState<TRecord>();
    }

    /// <summary>
    /// Applies a GridState change to the Broker and saves the state
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Result DispatchGridStateChange(UpdateGridRequest<TRecord> request)
    {
        this.GridState = new() { PageSize = request.PageSize, StartIndex = request.StartIndex, SortField = request.SortField, SortDescending = request.SortDescending };

        _gridStateStore.Dispatch(this.StateContextUid, this.GridState);

        return Result.Success();
    }

    /// <summary>
    /// Method called by QuickGrid to get the items
    /// </summary>
    /// <returns></returns>
    public async ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync()
        => await this.ItemsAsync();

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

    private async ValueTask<GridItemsProviderResult<TRecord>> ItemsAsync()
    {
        var result = GridItemsProviderResult.From<TRecord>(new List<TRecord>(), 0);

        this.LastResult = await Result<GridState<TRecord>>.Create(this.GridState)
            .MapToResultAsync(_entityProvider.GetItemsAsync)
            .TaskSideEffectAsync(
                success: (provider) => result = provider,
                failure: (ex) => this.LastResult = Result.Failure(ex.Message))
            .MapTaskToResultAsync();

        return result;
    }

    private void OnStateChanged(object? message)
    {
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
