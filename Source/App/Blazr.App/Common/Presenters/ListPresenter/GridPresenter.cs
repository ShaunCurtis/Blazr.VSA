/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Gallium;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public abstract class GridPresenter<TRecord>
    : IGridPresenter<TRecord>, IDisposable
    where TRecord : class, new()
{
    // Services
    protected readonly IMediator _dataBroker;
    protected readonly IMessageBus _messageBus;
    private readonly ScopedStateProvider _gridStateStore;

    public Guid StateContextUid { get; private set; } = Guid.NewGuid();
    public GridState<TRecord> GridState { get; private set; } = new();
    public IDataResult LastResult { get; protected set; } = DataResult.Failure("New");

    public event EventHandler<EventArgs>? StateChanged;

    public GridPresenter(IMediator mediator, IMessageBus messageBus, ScopedStateProvider scopedStateProvider)
    {
        _dataBroker = mediator;
        _messageBus = messageBus;
        _gridStateStore = scopedStateProvider;

        _messageBus.Subscribe<TRecord>(this.OnStateChanged);
    }

    /// <summary>
    /// Sets the State context for the Presenter and retrieves any saved GridState
    /// </summary>
    /// <param name="context"></param>
    public void SetContext(Guid context)
    {
        this.StateContextUid = context;
        if (_gridStateStore.TryGetState<GridState<TRecord>>(context, out GridState<TRecord>? state))
        {
            this.GridState = state;
            return;
        }

        this.GridState = new GridState<TRecord>();
    }

    /// <summary>
    /// Applies a GridState change to the Presenter and saves the state
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public IDataResult DispatchGridStateChange(UpdateGridRequest<TRecord> request)
    {
        this.GridState = new() { PageSize = request.PageSize, StartIndex = request.StartIndex, SortField = request.SortField, SortDescending = request.SortDescending };

        _gridStateStore.Dispatch(this.StateContextUid, this.GridState);

        return DataResult.Success();
    }

    protected abstract Task<Result<ListResult<TRecord>>> GetItemsAsync(GridState<TRecord> state);

    public async ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync()
    {
        var result = await this.GetItemsAsync(this.GridState);
        this.LastResult = result.ToDataResult;

        if (!result.HasSucceeded(out ListResult<TRecord> listResult))
            return GridItemsProviderResult.From<TRecord>(new List<TRecord>(), 0);

        // return a new GridItemsProviderResult created from the ListQueryResult
        return GridItemsProviderResult.From<TRecord>(listResult.Items.ToList(), listResult.TotalCount); ;
    }

    public void OnStateChanged(object? message)
    {
        this.StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _messageBus.UnSubscribe<TRecord>(this.OnStateChanged);
    }
}
