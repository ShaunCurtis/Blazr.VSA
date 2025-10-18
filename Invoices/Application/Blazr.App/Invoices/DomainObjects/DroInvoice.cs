/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App;

public record DroInvoice : CollectionRecord<DmoInvoice, DmoInvoiceItem>
{
    private readonly DroInvoice _baseRecord;
    
    public InvoiceId Id { get; private init; }

    public DroInvoice(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        : base(invoice, invoiceItems)
    {
        this.Id = invoice.Id;
        _baseRecord = this with { };
    }

    private DroInvoice(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems, DroInvoice baseRecord)
        : base(invoice, invoiceItems)
    {
        this.Id = baseRecord.Id;
        _baseRecord = baseRecord;
    }

    public Result<DroInvoice> ToResult => Result<DroInvoice>.Create(this);

    public Result<DroInvoice> Mutate(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => new DroInvoice(invoice, invoiceItems, _baseRecord).ToResult;
    
    public Result<DroInvoice> Mutate(DmoInvoice invoice)
        => new DroInvoice(invoice, this.Items, _baseRecord).ToResult;

    public Result<DroInvoice> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
        => new DroInvoice(this.Record, invoiceItems, _baseRecord).ToResult;

    public static Result<DroInvoice> ApplyEntityRules(DroInvoice invoiceRecord)
    {
        var total = invoiceRecord.Items.Sum(item => item.Amount.Value);

        if (invoiceRecord.Record.TotalAmount.Value == total)
            return Result<DroInvoice>.Success(invoiceRecord);

        var newInvoice = invoiceRecord.Record with { TotalAmount = new(total) };

        return DroInvoice.CreateAsResult(newInvoice, invoiceRecord.Items);
    }

    public static Result<DroInvoice> CheckEntityRules(DroInvoice invoiceRecord)
    {
        var total = invoiceRecord.Items.Sum(item => item.Amount.Value);

        return invoiceRecord.Record.TotalAmount.Value == total
            ? Result<DroInvoice>.Success(invoiceRecord)
            : Result<DroInvoice>.Failure("The Invoice Total Amount is incorrect.");
    }

    public static DroInvoice Create(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => new DroInvoice(invoice, invoiceItems);

    public static Result<DroInvoice> CreateAsResult(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => Result<DroInvoice>.Create(Create(invoice, invoiceItems));
}
