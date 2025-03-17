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
// It is designed to be inherited by a specific Broker for a specific data type
// Each specific child Broker will implement the GetItemsAsync method to retrieve the data  
// The Broker is responsible for managing the GridState and saving it to the ScopedStateProvider

// The template pattern for GetItemsAsync is:
//     protected override async Task<Result<ListResult<Dmoxxx>>> GetItemsAsync(GridState<Dmoxxx> state)
//    {
//        var listRequest = new xxxListRequest()
//        {
//            PageSize = state.PageSize,
//            StartIndex = state.StartIndex,
//            SortColumn = state.SortField,
//            SortDescending = state.SortDescending
//        };
//        var result = await _dataBroker.Send(listRequest);
//       return result;
//    }
//

public abstract class GridUIBroker<TRecord>
    : IGridUIBroker<TRecord>, IDisposable
    where TRecord : class, new()
{
    // Services
    protected readonly IMediator _dataBroker;
    protected readonly IMessageBus _messageBus;
    private readonly ScopedStateProvider _gridStateStore;

    public Guid StateContextUid { get; private set; } = Guid.NewGuid();
    public GridState<TRecord> GridState { get; private set; } = new();
    public IDataResult LastResult { get; protected set; } = DataResult.Success("New");

    public event EventHandler<EventArgs>? StateChanged;

    public GridUIBroker(IMediator mediator, IMessageBus messageBus, ScopedStateProvider scopedStateProvider)
    {
        _dataBroker = mediator;
        _messageBus = messageBus;
        _gridStateStore = scopedStateProvider;

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
    public IDataResult DispatchGridStateChange(UpdateGridRequest<TRecord> request)
    {
        this.GridState = new() { PageSize = request.PageSize, StartIndex = request.StartIndex, SortField = request.SortField, SortDescending = request.SortDescending };

        _gridStateStore.Dispatch(this.StateContextUid, this.GridState);

        return DataResult.Success();
    }

    /// <summary>
    /// Must be defined in any inheriting objects
    /// Creates a Mediator CustomerListRequest Request
    /// Basic Pattern is:
    ///   - Create Mediator ListRequest
    ///   - Send the request to the Mediator and return the result
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    protected abstract Task<Result<ListItemsProvider<TRecord>>> GetItemsAsync(GridState<TRecord> state);

    /// <summary>
    /// Method called by QuickGrid to get the items
    /// </summary>
    /// <returns></returns>
    public async ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync()
    {
        var result = await this.GetItemsAsync(this.GridState);
        this.LastResult = result.ToDataResult;

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
