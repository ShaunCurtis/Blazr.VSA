# UIBrokers

UI Brokers provide the interfaces between UI forms, such as the Weather Data Grid Form and the Core Domain logic.  They reside in the Presentation layer: the *Blazr.App.Presentation* namespace.

Think of the UI form as two halves:

1. The Form that contains all the UI elements and any logic that controls the UI.  Button events, enabling or hiding elements.
1. The UI Broker that contains all the data logic.  Getting data, handling paging requests, caching form state.

Here's the base Grid Form Component.

```csharp
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

    protected virtual string formTitle => this.FormTitle ?? $"List of {this.UIEntityProvider?.PluralDisplayName ?? "Items"}";
    protected string TableCss = "table table-sm table-striped table-hover border-bottom no-margin hide-blank-rows";
    protected string GridCss = "grid";

    protected async override Task OnInitializedAsync();
    private void OnStateChanged(object? sender, EventArgs e);
    public void Dispose();

    protected virtual async Task OnEditAsync(TKey id);
    protected virtual async Task OnViewAsync(TKey id);
    protected virtual async Task OnAddAsync();
    protected virtual Task OnDashboardAsync(TKey id);
}
```

The 

The `WeatherForecastGridForm`:

```csharp
@namespace Blazr.App.UI

@using Blazr.Cadmium.QuickGrid
@using Blazr.Cadmium.UI

@inherits GridFormBase<DmoWeatherForecast,WeatherForecastId>

<div class="d-flex flex-row mb-1 pt-1 border-bottom">
    <div class="flex-fill justify-content-start h4">
        @this.formTitle
    </div>
    <div class="justify-content-end">
        <UIButton class="btn btn-sm btn-primary" ClickEvent="() => OnAddAsync()">Add New Weather Forecast</UIButton>
    </div>
</div>

<BlazrPaginator State="Pagination" />

<div class="@this.GridCss" tabindex="-1">
    <QuickGrid Theme="None" TGridItem="DmoWeatherForecast" ItemsProvider="UIBroker.GetItemsAsync" Pagination="Pagination" @ref="this.quickGrid" Class="@this.TableCss">

        <SortedPropertyColumn SortField="Id" Class="nowrap-column" Sortable="true" Title="ID">
            @context.Id.Value.ToDisplayId()
        </SortedPropertyColumn>

        <PropertyColumn Title="Date" Property="@(c => c!.Date.Value)" Format="dd-MMM-yy" Sortable="true" IsDefaultSortColumn InitialSortDirection="SortDirection.Ascending" Align=Align.Start />

        <SortedPropertyColumn SortField="Temperature" Class="nowrap-column" Sortable="true" Align="Align.End" Title="Temp &deg;C">
            @context.Temperature.TemperatureC.ToString("F0")
        </SortedPropertyColumn>

        <SortedPropertyColumn SortField="Temperature" Class="nowrap-column" Sortable="true" Align="Align.End" Title="Temp &deg;F">
            @context.Temperature.TemperatureF.ToString("F0")
        </SortedPropertyColumn>

        <PropertyColumn Title="Summary" Sortable="true" Property="@(c => c!.Summary)" Align=Align.End />

        <TemplateColumn Class="nowrap-column no-max-width" Align="Align.End">
            <UIButtonGroup>
                <UIButton type="button" class="btn btn-sm btn-danger" ClickEvent="() => OnDeleteAsync(context.Id)">Delete</UIButton>
                <UIButton type="button" class="btn btn-sm btn-primary" ClickEvent="() => OnEditAsync(context.Id)">Edit</UIButton>
                <UIButton type="button" class="btn btn-sm btn-secondary" ClickEvent="() => OnViewAsync(context.Id)">View</UIButton>
            </UIButtonGroup>
        </TemplateColumn>

    </QuickGrid>
</div>

<BsModalDialog @ref=modalDialog />

@code {
    private Task OnDeleteAsync(WeatherForecastId id)
    {
        return Task.CompletedTask;
    }
}
```