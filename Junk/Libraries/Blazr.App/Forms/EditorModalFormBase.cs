/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation;
using Blazr.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazr.App.UI;

public abstract class EditorModalFormBase<TRecord, TKey, TEditContext, TEntityService>
    : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
    where TEditContext : class, IRecordEditContext<TRecord>, new()
    where TEntityService : class, IUIEntityProvider<TRecord>
{
    [Inject] protected IEditUIBrokerFactory UIBrokerFactory { get; set; } = default!;
    [Inject] protected IJSRuntime Js { get; set; } = default!;
    [Inject] protected IUIEntityProvider<TRecord> UIEntityService { get; set; } = default!;

    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }
    [Parameter, EditorRequired] public TKey Uid { get; set; } = default!;
    [Parameter] public bool LockNavigation { get; set; } = true;

    protected IEditUIBroker<TEditContext, TKey> UIBroker = default!;

    protected EditFormButtonsOptions editFormButtonsOptions = new();
    protected bool IsNewRecord => this.UIBroker.CommandState == CommandState.Add;
    protected string FormTitle => $"{this.UIEntityService.SingleDisplayName} Editor";

    protected async override Task OnInitializedAsync()
    {
        ArgumentNullException.ThrowIfNull(Uid);

        this.UIBroker = await this.UIBrokerFactory.GetAsync<TEditContext, TKey>(Uid);
        this.UIBroker.EditContext.OnFieldChanged += OnEditStateMayHaveChanged;
    }

    protected async Task OnSave()
    {
        await this.UIBroker.SaveItemAsync();
        await OnExit();
    }

    protected async Task OnDelete()
    {
        bool confirmed = await Js.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?");
        if (!confirmed)
            return;

        await this.UIBroker.DeleteItemAsync();
        await OnExit();
    }

    protected Task OnExit()
    {
        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }

    protected void OnEditStateMayHaveChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected async Task OnReset()
    {
        await this.UIBroker.ResetItemAsync();
        this.UIBroker.EditContext.Validate();
    }

    public void Dispose()
    {
        this.UIBroker.EditContext.OnFieldChanged -= OnEditStateMayHaveChanged;
    }
}
