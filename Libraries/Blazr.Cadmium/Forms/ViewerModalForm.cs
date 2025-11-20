/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
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
    protected Bool LastResult { get; set; } = Bool.Success();

    protected async override Task OnInitializedAsync()
    {
        // check we have the necessary parameter objects to function
        ArgumentNullException.ThrowIfNull(Uid);
        ArgumentNullException.ThrowIfNull(ModalDialog);

        await this.Load();

        _messageBus.Subscribe<TKey>(OnRecordChanged);
    }

    private async Task Load()
    {
        var result = await BoolT.Success(Uid)
             .ExecuteTransformAsync(UIConnector.RecordRequestAsync);

        this.LastResult = result.ToBoolT();

        this.Item = result.Output(exception => new TRecord());
    }

    protected virtual async void OnRecordChanged(object? sender)
    {
        if (sender is TKey key && this.Uid.Equals(key))
        {
            await this.Load();
            this.StateHasChanged();
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
