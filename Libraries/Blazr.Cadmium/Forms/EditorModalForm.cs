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
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Blazr.Cadmium.UI;

public abstract class EditorModalForm<TRecord, TRecordMutor, TKey>
    : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId, new()
    where TRecordMutor : class, IRecordMutor<TRecord>
{
    [Inject] private IJSRuntime Js { get; set; } = default!;
    [Inject] private IUIConnector<TRecord, TKey> UIConnector { get; set; } = default!;

    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }
    [Parameter, EditorRequired] public TKey Uid { get; set; } = new();
    [Parameter] public bool LockNavigation { get; set; } = true;

    protected Result LastResult { get; set; } = Result.Successful();
    protected TRecordMutor EditMutor { get; set; } = default!;
    protected EditContext EditContext { get; set; } = default!;

    protected EditFormButtonsOptions editFormButtonsOptions = new();
    protected bool IsNewRecord => this.Uid.IsNew;
    protected string FormTitle => $"{this.UIConnector.SingleDisplayName} Editor";
    protected bool Loading => !this.Loaded;
    protected bool Loaded;

    protected RecordState State => this.Uid.IsNew
            ? RecordState.NewState
            : RecordState.CleanState;

    protected async override Task OnInitializedAsync()
    {
        // Check we have a Uid.  If not then we can't proceed so throw an exception
        ArgumentNullException.ThrowIfNull(Uid);

        var result = await this.UIConnector.RecordRequestAsync(this.Uid);

        LastResult = result.AsResult;

        this.EditMutor = result
            .Map(record => (TRecordMutor)this.UIConnector.GetRecordMutor(record))
            .Write(defaultValue: default!);

        this.EditContext = new EditContext(EditMutor);

        Loaded = true;

        this.EditContext.OnFieldChanged += OnEditStateMayHaveChanged;
    }

    protected virtual async Task OnSave()
    {
        this.LastResult = await this.UIConnector.RecordCommandAsync(this.EditMutor.Record, this.EditMutor.State)
            .AsResultAsync();

        this.OnExit();
    }

    protected virtual async Task OnDelete()
    {
        if (await ConfirmAsync())
            return;

        this.LastResult = await this.UIConnector.RecordCommandAsync(this.EditMutor.Record, RecordState.DeletedState)
            .AsResultAsync();

        this.OnExit();
    }

    protected async Task<bool> ConfirmAsync()
        => await Js.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?");
 
    protected virtual void OnExit()
        => ModalDialog?.Close(new ModalResult());

    protected void OnEditStateMayHaveChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected virtual Task OnReset()
    {
        this.EditMutor.Reset();
        this.EditContext = new EditContext(EditMutor);
        this.EditContext.Validate();

        return Task.CompletedTask;
    }

    public void Dispose()
        => this.EditContext.OnFieldChanged -= OnEditStateMayHaveChanged;
}
