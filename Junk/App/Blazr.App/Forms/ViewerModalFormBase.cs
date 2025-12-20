/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation;
using Blazr.UI;
using Microsoft.AspNetCore.Components;

namespace Blazr.App.UI;

/// <summary>
/// The boilerplate base class for Modal View Forms 
/// </summary>
/// <typeparam name="TRecord"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract partial class ViewerModalFormBase<TRecord, TKey> : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    [Inject] protected IReadUIBrokerFactory UIBrokerFactory { get; set; } = default!;
    [Inject] protected IUIEntityProvider<TRecord> UIEntityService { get; set; } = default!;

    [Parameter] public TKey Uid { get; set; } = default!;
    [CascadingParameter] protected IModalDialog? ModalDialog { get; set; }

    protected IReadUIBroker<TRecord, TKey> UIBroker { get; set; } = default!;
    protected string FormTitle => $"{this.UIEntityService.SingleDisplayName} Viewer";

    protected async override Task OnInitializedAsync()
    {
        // check we have the necessary parameter objects to function
        ArgumentNullException.ThrowIfNull(Uid);
        ArgumentNullException.ThrowIfNull(ModalDialog);

        this.UIBroker = await this.UIBrokerFactory.GetAsync<TRecord, TKey>(this.Uid);

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
        this.UIBroker.RecordChanged -= OnRecordChanged;
        this.UIBroker.Dispose();
    }
}
