/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Gallium;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

// This is the boilerplate code for any GridUIBroker
// It is an abstract class that implements the IGridUIBroker interface
// It provides the basic functionality for a Grid Broker

public class GridUIBroker<TRecord, TKey>
    : IGridUIBroker<TRecord>, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    // Services
    protected readonly IMediator _dataBroker;
    protected readonly IMessageBus _messageBus;
    private readonly ScopedStateProvider _gridStateStore;
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;

    public Guid StateContextUid { get; private set; } = Guid.NewGuid();
    public GridState<TRecord> GridState { get; private set; } = new();
    public IResult LastResult { get; protected set; } = Result.Success();

    public event EventHandler<EventArgs>? StateChanged;
    
    public GridUIBroker(IMediator mediator, IMessageBus messageBus, IEntityProvider<TRecord, TKey> entityProvider, ScopedStateProvider scopedStateProvider)
    {
        _dataBroker = mediator;
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
    public IResult DispatchGridStateChange(UpdateGridRequest<TRecord> request)
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
    {
        var result = await _entityProvider.ListRequest.Invoke(this.GridState);
        this.LastResult = result;

        if (result.HasNotSucceeded(out ListItemsProvider<TRecord>? listResult))
            return GridItemsProviderResult.From<TRecord>(new List<TRecord>(), 0);

        // return a new GridItemsProviderResult created from the ListQueryResult
        return GridItemsProviderResult.From<TRecord>(listResult.Items.ToList(), listResult.TotalCount); ;
    }

    private void OnStateChanged(object? message)
    {
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _messageBus.UnSubscribe<TRecord>(this.OnStateChanged);
    }
}
