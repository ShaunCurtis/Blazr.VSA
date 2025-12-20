/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.Antimony.Mediator;

namespace Blazr.App.Invoice.Core;

public sealed partial class InvoiceEntity
{
    private readonly IMediatorBroker _mediator;
    private readonly List<InvoiceItemContext> _items = new List<InvoiceItemContext>();
    private readonly List<InvoiceItemContext> _itemsBin = new List<InvoiceItemContext>();
    private readonly InvoiceContext Invoice;
    private readonly List<DmoInvoiceItem> _baseItems = new();
    private readonly DmoInvoice _baseInvoice;
    private bool _processing;
    private IEnumerable<DsrInvoiceItem> _itemRecords => _items.Select(item => item.AsRecord);

    public DsrInvoice InvoiceRecord 
        => this.Invoice.AsRecord(_itemRecords.ToList());

    public IEnumerable<DsrInvoiceItem> InvoiceItems
        => _items.Select(item => item.AsRecord).AsEnumerable();

    public IEnumerable<DsrInvoiceItem> InvoiceItemsBin
        => _itemsBin.Select(item => item.AsRecord).AsEnumerable();

    public bool IsDirty
        => this.Invoice.IsDirty ? true : _items.Any(item => item.IsDirty);

    public event EventHandler<InvoiceId>? StateHasChanged;

    public InvoiceEntity(IMediatorBroker mediator, DmoInvoice invoice, IEnumerable<DmoInvoiceItem> items)
    {
        _mediator = mediator;
        _baseInvoice = invoice;
        _baseItems.AddRange(items);

        // Create new InvoiceContext for the Invoice
        this.Invoice = new InvoiceContext(invoice);

        // Detect if the Invoice is a new record
        if (invoice.Id.IsDefault)
            this.Invoice.State = CommandState.Add;

        // Create new InvoiceItemContexts for the InvoiceItems
        foreach (var item in items)
        {
            _items.Add(new InvoiceItemContext(item with { }));
        }
    }

    private void ItemUpdated(InvoiceItemContext item)
    {
        this.ApplyRules();
    }

    private void InvoiceUpdated()
    {
        this.ApplyRules();
    }

    private void ApplyRules()
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
}
