namespace Blazr.App.Core;

public sealed partial class InvoiceComposite
{
    private readonly List<InvoiceItem> _items = new List<InvoiceItem>();
    private readonly List<InvoiceItem> _itemsBin = new List<InvoiceItem>();
    private readonly Invoice Invoice;
    private bool _processing;
    private IEnumerable<InvoiceItemRecord> _itemRecords => _items.Select(item => item.AsRecord);

    public InvoiceRecord InvoiceRecord 
        => this.Invoice.AsRecord(_itemRecords.ToList());

    public IEnumerable<InvoiceItemRecord> InvoiceItems
        => _items.Select(item => item.AsRecord).AsEnumerable();

    public IEnumerable<InvoiceItemRecord> InvoiceItemsBin
        => _itemsBin.Select(item => item.AsRecord).AsEnumerable();

    public bool IsDirty
        => this.Invoice.IsDirty ? true : _items.Any(item => item.IsDirty);

    public event EventHandler<InvoiceId>? StateHasChanged;

    public InvoiceComposite(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> items)
    {
        // We create new records for the Invoice and InvoiceItems
        this.Invoice = new Invoice(invoice);

        // Detect if the Invoice is a new record
        if (invoice.Id.IsDefault)
            this.Invoice.State = CommandState.Add;

        foreach (var item in items)
        {
            _items.Add(new InvoiceItem(item with { }));
        }
    }

    private void ItemUpdated(InvoiceItem item)
    {
        this.Process();
    }

    private void InvoiceUpdated()
    {
        this.Process();
    }

    private void Process()
    {
        // prevent calling oneself
        if (_processing)
            return;

        _processing = true;
        decimal total = 0m;
        foreach (var item in _items)
            total += item.Amount;

        if (total != this.InvoiceRecord.Record.TotalAmount)
        {
            this.Invoice.Update(this.InvoiceRecord.Record with { TotalAmount = total });
        }
        this.StateHasChanged?.Invoke(this, this.InvoiceRecord.Record.Id);

        _processing = false;
    }

    public static InvoiceComposite Default
        => new InvoiceComposite(InvoiceEntityProvider.DefaultRecord, Enumerable.Empty<DmoInvoiceItem>());
}
