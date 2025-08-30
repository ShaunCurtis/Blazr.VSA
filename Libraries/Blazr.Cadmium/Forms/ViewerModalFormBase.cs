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

namespace Blazr.Cadmium.UI;

/// <summary>
/// The boilerplate base class for Modal View Forms 
/// </summary>
/// <typeparam name="TRecord"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract partial class ViewerModalFormBase<TRecord, TKey> : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    [Inject] protected IUIEntityProvider<TRecord, TKey> UIEntityProvider { get; set; } = default!;

    [Parameter] public TKey Uid { get; set; } = default!;
    [CascadingParameter] protected IModalDialog? ModalDialog { get; set; }

    protected IReadUIBroker<TRecord, TKey> UIBroker { get; private set; } = default!;
    protected string FormTitle => $"{this.UIEntityProvider.SingleDisplayName} Viewer";

    protected async override Task OnInitializedAsync()
    {
        // check we have the necessary parameter objects to function
        ArgumentNullException.ThrowIfNull(Uid);
        ArgumentNullException.ThrowIfNull(ModalDialog);

        this.UIBroker = await this.UIEntityProvider.GetReadUIBrokerAsync(this.Uid);

        this.UIBroker.RecordChanged += OnRecordChanged;
    }

    protected void OnRecordChanged(object? sender, EventArgs e)
    {
        this.StateHasChanged();
    }

    protected Task OnExit()
    {
        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if(UIBroker is null)
            return;

        this.UIBroker.RecordChanged -= OnRecordChanged;
        this.UIBroker.Dispose();
    }
}
