/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Presentation;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Blazr.Gallium;
using Blazr.Uranium;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium.UI;

public abstract class GridForm<TRecord, TKey> : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IUIConnector<TRecord, TKey> UIConnector { get; set; } = default!;
    [Inject] private ScopedStateProvider _gridStateStore { get; set; } = default!;
    [Inject] private IMessageBus _messageBus { get; set; } = default!;

    [Parameter] public string? FormTitle { get; set; }
    [Parameter] public Guid GridContextId { get; set; } = Guid.NewGuid();
    [Parameter] public int PageSize { get; set; } = 15;
    [Parameter] public bool ResetGridContext { get; set; }

    protected IModalDialog modalDialog = default!;
    protected QuickGrid<TRecord> quickGrid = default!;
    protected PaginationState Pagination = new PaginationState { ItemsPerPage = 10 };
    protected GridState<TRecord> GridState = new();
    protected Result LastResult = Result.Successful();

    protected string formTitle => this.FormTitle ?? $"List of {this.UIConnector?.PluralDisplayName ?? "Items"}";

    protected readonly string TableCss = "table table-sm table-striped table-hover border-bottom no-margin hide-blank-rows";
    protected readonly string GridCss = "grid";

    protected async override Task OnInitializedAsync()
    {
        // Set the paginator page size
        this.Pagination.ItemsPerPage = this.PageSize;

        // Get the current grid state from the store if one exists
        this.LastResult = this.GetGridState
            .Match(successAction: state => this.GridState = state)
            .AsResult;

        // Subscribe to record change messages so we can refresh the grid if a record is changed
        _messageBus.Subscribe<TKey>(OnRecordChanged);

        // Set the current page index in the pager.
        // This will trigger the GetItemsAsync method to be called with the correct page index.
        await Pagination.SetCurrentPageIndexAsync(this.GridState.Page);

        // Make sure we yield so we have the first UI render
        // before testing the modalDialog and quickGrid components exist in the form
        // We can't trust previous waits to have yielded.
        await Task.Yield();

        // We have rendered at this poinr so we can
        // check the modalDialog and quickGrid components exist in the form
        ArgumentNullException.ThrowIfNull(this.modalDialog);
        ArgumentNullException.ThrowIfNull(this.quickGrid);
    }

    private void SetLastResult(Result result) => this.LastResult = result;

    protected Result<GridState<TRecord>> SetGridState(UpdateGridRequest<TRecord> request)
        => _gridStateStore.Dispatch(request.ToGridState(this.GridContextId));

    protected Result<GridState<TRecord>> GetGridState
        => this.ResetGridContext
            ? this.ResetGridState()
            : _gridStateStore.GetState<GridState<TRecord>>(GridContextId);

    protected Result<GridState<TRecord>> ResetGridState()
        => _gridStateStore.Dispatch(new GridState<TRecord>
        {
            Key = this.GridContextId,
            PageSize = this.PageSize,
            StartIndex = 0,
            SortField = null,
            SortDescending = false
        });

    protected async ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync(GridItemsProviderRequest<TRecord> gridRequest)
    {
        var result = await ResultT.Successful(gridRequest
             .ConvertToUpdateGridRequest())
             .Bind(this.SetGridState)
             .BindAsync(UIConnector.GetItemsAsync);

        LastResult = result.AsResult;

        return result
            .Write(defaultValue: GridItemsProviderResult.From<TRecord>(new List<TRecord>(), 0));
    }

    protected virtual async Task OnEditAsync(TKey id)
    {
        ArgumentNullException.ThrowIfNull(this.UIConnector.EditForm);

        var options = new ModalOptions() { ModalDialogType = this.UIConnector.EditForm };
        options.ControlParameters.Add("Uid", id);

        await modalDialog.ShowAsync(options);
    }

    protected virtual async Task OnViewAsync(TKey id)
    {
        ArgumentNullException.ThrowIfNull(this.UIConnector.ViewForm);

        var options = new ModalOptions() { ModalDialogType = this.UIConnector.ViewForm };
        options.ControlParameters.Add("Uid", id);

        await modalDialog.ShowAsync(options);
    }

    protected virtual async Task OnAddAsync()
    {
        ArgumentNullException.ThrowIfNull(this.UIConnector.EditForm);

        // we don't set UId, so it will be default telling the edit this is a new record
        var options = new ModalOptions() { ModalDialogType = this.UIConnector.EditForm };

        await modalDialog.ShowAsync(options);
    }

    protected virtual Task OnDashboardAsync(TKey id)
    {
        this.NavManager.NavigateTo($"{this.UIConnector.Url}/dash/{id.ToString()}");

        return Task.CompletedTask;
    }

    private void OnRecordChanged(object? sender)
    {
        this.InvokeAsync(quickGrid.RefreshDataAsync);
    }

    public void Dispose()
    {
        _messageBus.UnSubscribe<TKey>(OnRecordChanged);
    }
}