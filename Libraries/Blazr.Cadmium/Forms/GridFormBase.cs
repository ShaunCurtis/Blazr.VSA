/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Presentation;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Blazr.Uranium;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium.UI;

public abstract partial class GridFormBase<TRecord, TKey> : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IUIEntityProvider<TRecord, TKey> UIEntityProvider { get; set; } = default!;

    [Parameter] public string? FormTitle { get; set; }
    [Parameter] public Guid GridContextId { get; set; } = Guid.NewGuid();
    [Parameter] public int PageSize { get; set; } = 15;
    [Parameter] public bool ResetGridContext { get; set; }

    protected IGridUIBroker<TRecord> UIBroker { get; private set; } = default!;
    protected IModalDialog modalDialog = default!;
    protected QuickGrid<TRecord> quickGrid = default!;
    protected PaginationState Pagination = new PaginationState { ItemsPerPage = 10 };
    //protected Expression<Func<TRecord, bool>>? DefaultFilter { get; set; } = null;

    protected virtual string formTitle => this.FormTitle ?? $"List of {this.UIEntityProvider?.PluralDisplayName ?? "Items"}";
    protected string TableCss = "table table-sm table-striped table-hover border-bottom no-margin hide-blank-rows";
    protected string GridCss = "grid";

    protected async override Task OnInitializedAsync()
    {
        this.Pagination.ItemsPerPage = this.PageSize;

        // If we are resetting the grid context, then we need to reset the saved grid state
        UIBroker = ResetGridContext
            ? await this.UIEntityProvider.GetGridUIBrokerAsync(this.GridContextId, new UpdateGridRequest<TRecord>(0, this.PageSize, false, null))
            : UIBroker = await this.UIEntityProvider.GetGridUIBrokerAsync(this.GridContextId);

        this.UIBroker.StateChanged += OnStateChanged;

        // Set the current page index in the pager.
        // This will trigger the GetItemsAsync method to be called with the correct page index.
        await Pagination.SetCurrentPageIndexAsync(this.UIBroker.GridState.Page);

        // Make sure we yield so we have the first UI render
        // before testing the modalDialog and quickGrid components exist in the form
        // We can't trust previous waits to have yielded.
        await Task.Yield();

        // Check the modalDialog and quickGrid components exist in the form
        ArgumentNullException.ThrowIfNull(this.modalDialog);
        ArgumentNullException.ThrowIfNull(this.quickGrid);
    }

    protected virtual async Task OnEditAsync(TKey id)
    {
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        ArgumentNullException.ThrowIfNull(this.UIEntityProvider.EditForm);

        await modalDialog.ShowAsync(this.UIEntityProvider.EditForm, options);
    }

    protected virtual async Task OnViewAsync(TKey id)
    {
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        ArgumentNullException.ThrowIfNull(this.UIEntityProvider.ViewForm);

        await modalDialog.ShowAsync(this.UIEntityProvider.ViewForm, options);
    }

    protected virtual async Task OnAddAsync()
    {
        var options = new ModalOptions();
        // we don't set UId, so it will be default telling the edit this is a new record

        ArgumentNullException.ThrowIfNull(this.UIEntityProvider.EditForm);

        await modalDialog.ShowAsync(this.UIEntityProvider.EditForm, options);
    }

    protected virtual Task OnDashboardAsync(TKey id)
    {
        this.NavManager.NavigateTo($"{this.UIEntityProvider.Url}/dash/{id.ToString()}");

        return Task.CompletedTask;
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        this.InvokeAsync(quickGrid.RefreshDataAsync);
    }

    public void Dispose()
    {
        if (this.UIBroker is not null)
            this.UIBroker.StateChanged -= OnStateChanged;
    }
}
