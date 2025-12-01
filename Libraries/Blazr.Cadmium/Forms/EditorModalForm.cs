/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.Extensions;
using Blazr.Cadmium.Presentation;
using Blazr.Diode;
using Blazr.Manganese;
using Blazr.Uranium;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Blazr.Cadmium.UI;

public abstract class EditorModalForm<TRecord, TRecordMutor, TKey>
    : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
    where TRecordMutor : class, IRecordMutor<TRecord>
{
    [Inject] private IJSRuntime Js { get; set; } = default!;
    [Inject] private IUIConnector<TRecord, TKey> UIConnector { get; set; } = default!;

    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }
    [Parameter, EditorRequired] public TKey Uid { get; set; } = default!;
    [Parameter] public bool LockNavigation { get; set; } = true;

    protected EditState State { get; set; } = EditState.Clean;
    protected Return LastResult { get; set; } = Return.Success();
    protected TRecordMutor EditMutor { get; set; } = default!;
    protected EditContext EditContext { get; set; } = default!;

    protected EditFormButtonsOptions editFormButtonsOptions = new();
    protected bool IsNewRecord => this.State == EditState.New;
    protected string FormTitle => $"{this.UIConnector.SingleDisplayName} Editor";
    protected bool Loading => !this.Loaded;
    protected bool Loaded;

    protected async override Task OnInitializedAsync()
    {
        ArgumentNullException.ThrowIfNull(Uid);
        
        this.State = this.Uid.IsDefault
            ? EditState.New
            : EditState.Clean;

        var result = await this.UIConnector.RecordRequestAsync(this.Uid);

        var record = result.Write(default!);

        this.EditMutor = (TRecordMutor)this.UIConnector.GetRecordMutor(record);
        this.EditContext = new EditContext(EditMutor);

        Loaded = true;

        this.EditContext.OnFieldChanged += OnEditStateMayHaveChanged;
    }

    protected virtual async Task OnSave()
    {
        var stateRecord = this.EditMutor.ToStateRecord();

        this.LastResult = (await this.UIConnector.RecordCommandAsync(stateRecord))
            .ToReturn();

        this.OnExit();
    }

    protected virtual async Task OnDelete()
    {
        if ((await ConfirmAsync()).Failed)
            return;

        var stateRecord = StateRecord<TRecord>.Create(this.EditMutor.Mutate(), EditState.Deleted);

        this.LastResult = (await this.UIConnector.RecordCommandAsync(stateRecord))
            .ToReturn();

        this.OnExit();
    }

    protected async Task<Return> ConfirmAsync()
        => await Js.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?")
            ? Return.Success()
            : Return.Failure();

    protected virtual void OnExit()
        => ModalDialog?.Close(new ModalResult());

    protected void OnEditStateMayHaveChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected virtual Task OnReset()
    {
        //TODO - don't we need to reset the mutor???
        // Create a new EditContext - will reset and rebuild the whole Edit Form and validate
        this.EditContext = new EditContext(EditMutor);
        this.EditContext.Validate();

        return Task.CompletedTask;
    }

    public void Dispose()
        => this.EditContext.OnFieldChanged -= OnEditStateMayHaveChanged;
}
