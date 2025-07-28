/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium.Presentation;

public interface IGridUIBroker<TRecord>
    where TRecord : class, new()
{
    public Guid StateContextUid { get; }
    public GridState<TRecord> GridState { get; }
    public Result LastResult { get; }

    public event EventHandler<EventArgs>? StateChanged;

    public Result SetContext(Guid context);
    public Result SetContext(Guid context, UpdateGridRequest<TRecord> resetGridRequest);
    public ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync(GridItemsProviderRequest<TRecord> gridRequest);
    public Result<GridState<TRecord>>Dispatch(UpdateGridRequest<TRecord> request);
}
