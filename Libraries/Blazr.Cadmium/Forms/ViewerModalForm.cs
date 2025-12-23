/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Presentation;
using Blazr.Diode;
using Blazr.Gallium;
using Blazr.Uranium;
using Microsoft.AspNetCore.Components;

namespace Blazr.Cadmium.UI;

/// <summary>
/// The boilerplate base class for Modal View Forms 
/// </summary>
/// <typeparam name="TRecord"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract partial class ViewerModalForm<TRecord, TKey> : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    [Inject] protected IUIConnector<TRecord, TKey> UIConnector { get; set; } = default!;
    [Inject] private IMessageBus _messageBus { get; set; } = default!;

    [Parameter] public TKey Uid { get; set; } = default!;
    [CascadingParameter] protected IModalDialog? ModalDialog { get; set; }

    protected string FormTitle => $"{this.UIConnector.SingleDisplayName} Viewer";

    protected TRecord Item { get; set; } = new TRecord();
    protected Return LastResult { get; set; } = Return.Success();

    protected async override Task OnInitializedAsync()
    {
        // check we have the necessary parameter objects to function
        ArgumentNullException.ThrowIfNull(Uid);
        ArgumentNullException.ThrowIfNull(ModalDialog);

        await this.LoadAsync();

        _messageBus.Subscribe<TKey>(OnRecordChanged);
    }

    private async Task LoadAsync()
    {
        // Load the record
        // if it can't be found, create a new blank record
        this.Item = await UIConnector.RecordRequestAsync(Uid)
             .SetReturnAsync(this.SetLastResult)
             .WriteAsync(defaultValue: new TRecord());
    }

    private void SetLastResult(Return result) => this.LastResult = result;

    protected virtual async void OnRecordChanged(object? sender)
    {
        // Check to see if the changed record is the one we are viewing
        // if so, reload it
        if (sender is TKey key && this.Uid.Equals(key))
        {
            await this.LoadAsync();
            await this.InvokeAsync(this.StateHasChanged);
        }
    }

    protected Task OnExit()
    {
        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _messageBus.UnSubscribe<TKey>(OnRecordChanged);
    }
}
