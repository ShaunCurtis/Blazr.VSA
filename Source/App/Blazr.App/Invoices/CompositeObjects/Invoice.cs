/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class Invoice : IDisposable
{
    private Action<Invoice>? UpdateCallback;

    public CommandState State { get; set; }
        = CommandState.None;

    public DmoInvoice Record { get; private set; }

    public bool IsDirty
        => this.State != CommandState.None;

    public InvoiceRecord AsRecord
        => new(this.Record, this.State);

    public Invoice(DmoInvoice item, Action<Invoice> callback, bool isNew = false)
    {
        this.Record = item;
        this.UpdateCallback = callback;

        if (isNew || item.Id.IsDefault)
            this.State = CommandState.Add;
    }

    public InvoiceId Id => this.Record.Id;

    public void Update(DmoInvoice invoice)
    {
        this.Record = invoice;
        this.State = this.State.AsDirty;
        UpdateCallback?.Invoke(this);
    }

    public void Dispose()
    {
        this.UpdateCallback = null;
    }
}
