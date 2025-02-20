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
    [Inject] protected IReadPresenterFactory PresenterFactory { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IUIEntityProvider<TRecord> UIEntityService { get; set; } = default!;
    [Inject] protected IMessageBus MessageBus { get; set; } = default!;

    [Parameter] public TKey Uid { get; set; } = default!;
    [Parameter] public bool HideHeader { get; set; }
    [Parameter] public bool HideFooter { get; set; }
    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }

    protected IReadPresenter<TRecord, TKey> Presenter { get; set; } = default!;
    protected string ExitUrl { get; set; } = "/";

    protected async override Task OnInitializedAsync()
    {
        ArgumentNullException.ThrowIfNull(Uid);

        this.MessageBus.Subscribe<TRecord>(this.OnRecordChanged);

        this.Presenter = await this.PresenterFactory.GetPresenterAsync<TRecord, TKey>(this.Uid);
    }

    private async void OnRecordChanged(object? message)
    {
        await this.Presenter.LoadAsync(Uid);
        this.StateHasChanged();
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
