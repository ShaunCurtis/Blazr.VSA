/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public interface IGridPresenter<TRecord>
    where TRecord : class, new()
{
    public Guid StateContextUid { get; }
    public GridState<TRecord> GridState { get; }
    public IDataResult LastResult { get; }

    public void SetContext(Guid context);
    public ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync();
    public IDataResult DispatchGridStateChange(UpdateGridRequest<TRecord> request);
}
