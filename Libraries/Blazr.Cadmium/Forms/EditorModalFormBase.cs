/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.Presentation;
using Blazr.Diode;
using Blazr.Uranium;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazr.Cadmium.UI;

public abstract class EditorModalFormBase<TRecord, TRecordMutor, TKey>
    : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
    where TRecordMutor : class, IRecordMutor<TRecord>
{
    [Inject] protected IJSRuntime Js { get; set; } = default!;
    [Inject] protected IUIEntityProvider<TRecord,TKey> UIEntityProvider { get; set; } = default!;

    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }
    [Parameter, EditorRequired] public TKey Uid { get; set; } = default!;
    [Parameter] public bool LockNavigation { get; set; } = true;

    protected IEditUIBroker<TRecord, TRecordMutor, TKey> UIBroker = default!;

    protected EditFormButtonsOptions editFormButtonsOptions = new();
    protected bool IsNewRecord => this.UIBroker.State == EditState.New;
    protected string FormTitle => $"{this.UIEntityProvider.SingleDisplayName} Editor";

    protected async override Task OnInitializedAsync()
    {
        ArgumentNullException.ThrowIfNull(Uid);

        this.UIBroker = await this.UIEntityProvider.GetEditUIBrokerAsync<TRecordMutor>(Uid);
        this.UIBroker.EditContext.OnFieldChanged += OnEditStateMayHaveChanged;
    }

    protected async Task OnSave()
    {
        await this.UIBroker.SaveAsync();
        await OnExit();
    }

    protected async Task OnDelete()
    {
        bool confirmed = await Js.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?");
        if (!confirmed)
            return;

        await this.UIBroker.DeleteAsync();
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
        await this.UIBroker.ResetAsync();
        this.UIBroker.EditContext.Validate();
    }

    public void Dispose()
    {
        if (UIBroker is null)
            return;

        this.UIBroker.EditContext.OnFieldChanged -= OnEditStateMayHaveChanged;
    }
}
