/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class InvoiceItem : IDisposable
{
    private Action<InvoiceItem>? UpdateCallback;

    public CommandState State { get; set; }
        = CommandState.None;

    public DmoInvoiceItem Record { get; private set; }

    public bool IsDirty
        => this.State != CommandState.None;

    public InvoiceItemRecord AsRecord
        => new(this.Record, this.State);
    public InvoiceItem(DmoInvoiceItem item, Action<InvoiceItem> callback, bool isNew = false)
    {
        this.Record = item;
        this.UpdateCallback = callback;

        if (isNew || item.Id.IsDefault)
            this.State = CommandState.Add;
    }

    public InvoiceItemId Id => this.Record.Id;
    public string Description => this.Record.Description;
    public decimal Amount => this.Record.Amount;

    public void Update(DmoInvoiceItem invoiceItem)
    {
        this.Record = invoiceItem;
        this.State = this.State.AsDirty;
        UpdateCallback?.Invoke(this);
    }

    public void Dispose()
    {
        this.UpdateCallback = null;
    }
}
