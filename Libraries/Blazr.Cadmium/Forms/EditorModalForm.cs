﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
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

    private EditState State { get; set; } = EditState.Clean;
    protected Result LastResult { get; set; } = Result.Success();
    protected TRecordMutor EditMutor { get; set; } = default!;
    protected EditContext EditContext { get; set; } = default!;

    protected EditFormButtonsOptions editFormButtonsOptions = new();
    protected bool IsNewRecord => this.State == EditState.New;
    protected string FormTitle => $"{this.UIConnector.SingleDisplayName} Editor";
    protected bool Loading => !_loaded;
    private bool _loaded;

    protected async override Task OnInitializedAsync()
    {
        ArgumentNullException.ThrowIfNull(Uid);
        
        this.State = this.Uid.IsDefault
            ? EditState.New
            : EditState.Clean;

        var result = await this.UIConnector.RecordRequestAsync(this.Uid);

        var record = result.OutputValue(ex => default!);

        this.EditMutor = (TRecordMutor)this.UIConnector.GetRecordMutor(record);
        this.EditContext = new EditContext(EditMutor);

        _loaded = true;

        this.EditContext.OnFieldChanged += OnEditStateMayHaveChanged;
    }

    protected async Task OnSave()
        => LastResult = await this.UIConnector.RecordCommandAsync(StateRecord<TRecord>.Create(this.EditMutor.ToRecord(), this.State.AsDirty))
            .ToResultAsync()
            .ExecuteSideEffectAsync(this.OnExit);

    protected async Task OnDelete()
    {
        bool confirmed = await Js.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?");
        if (!confirmed)
            return;

        this.LastResult = await this.UIConnector.RecordCommandAsync(StateRecord<TRecord>.Create(this.EditMutor.ToRecord(), EditState.Deleted))
            .ToResultAsync()
            .ExecuteSideEffectAsync(this.OnExit);
    }

    protected void OnExit()
        => ModalDialog?.Close(new ModalResult());

    protected void OnEditStateMayHaveChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected Task OnReset()
    {
        // Create a new EditContext - will reset and rebuild the whole Edit Form snd validate
        this.EditContext = new EditContext(EditMutor);
        this.EditContext.Validate();

        return Task.CompletedTask;
    }

    public void Dispose()
        => this.EditContext.OnFieldChanged -= OnEditStateMayHaveChanged;
}
