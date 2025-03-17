/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation;
using Blazr.Gallium;
using Blazr.UI;
using Microsoft.AspNetCore.Components;

namespace Blazr.App.UI;

/// <summary>
/// The boilerplate base class for View Forms 
/// </summary>
/// <typeparam name="TRecord"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract partial class ViewerFormBase<TRecord, TKey> : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    [Inject] protected IReadUIBrokerFactory UIBrokerFactory { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IUIEntityProvider<TRecord> UIEntityService { get; set; } = default!;
    [Inject] protected IMessageBus MessageBus { get; set; } = default!;

    [Parameter] public TKey Uid { get; set; } = default!;
    [Parameter] public bool HideHeader { get; set; }
    [Parameter] public bool HideFooter { get; set; }
    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }

    protected IReadUIBroker<TRecord, TKey> UIBroker { get; set; } = default!;
    protected string ExitUrl { get; set; } = "/";

    protected async override Task OnInitializedAsync()
    {
        // check we have the necessary parameter objects to function
        ArgumentNullException.ThrowIfNull(Uid);

        // subscribe for any record changes
        this.MessageBus.Subscribe<TRecord>(this.OnRecordChanged);

        //
        this.UIBroker = await this.UIBrokerFactory.GetAsync<TRecord, TKey>(this.Uid);
    }

    private async void OnRecordChanged(object? record)
    {
        // test to see if we have a key of the same type
        // if so and it doesn't match the current key, we dont need to do anything
        // if we don't have a key, we need to load the record just in case
        if (record is TKey value)
        {
            if (!this.Uid.Equals(value))
                return;
        }

        await this.UIBroker.LoadAsync(Uid);
        this.StateHasChanged();
        return;
    }

protected Task OnExit()
    {
        if (this.ModalDialog is null)
            this.NavManager.NavigateTo(this.ExitUrl);

        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        this.MessageBus.UnSubscribe<TRecord>(this.OnRecordChanged);
    }
}
