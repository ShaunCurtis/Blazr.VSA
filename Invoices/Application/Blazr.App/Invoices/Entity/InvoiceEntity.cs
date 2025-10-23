/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App;

public record InvoiceEntity : CollectionRecord<DmoInvoice, DmoInvoiceItem>
{
    private readonly InvoiceEntity _baseRecord;
    
    public InvoiceId Id { get; private init; }

    public InvoiceEntity(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        : base(invoice, invoiceItems)
    {
        this.Id = invoice.Id;
        _baseRecord = this with { };
    }

    private InvoiceEntity(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems, InvoiceEntity baseRecord)
        : base(invoice, invoiceItems)
    {
        this.Id = baseRecord.Id;
        _baseRecord = baseRecord;
    }

    public Result<InvoiceEntity> ToResult => Result<InvoiceEntity>.Create(this);

    public Result<InvoiceEntity> Mutate(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => new InvoiceEntity(invoice, invoiceItems, _baseRecord).ToResult;
    
    public Result<InvoiceEntity> Mutate(DmoInvoice invoice)
        => new InvoiceEntity(invoice, this.Items, _baseRecord).ToResult;

    public Result<InvoiceEntity> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
        => new InvoiceEntity(this.Record, invoiceItems, _baseRecord).ToResult;

    public static Result<InvoiceEntity> ApplyEntityRules(InvoiceEntity invoiceRecord)
    {
        var total = invoiceRecord.Items.Sum(item => item.Amount.Value);

        if (invoiceRecord.Record.TotalAmount.Value == total)
            return Result<InvoiceEntity>.Success(invoiceRecord);

        var newInvoice = invoiceRecord.Record with { TotalAmount = new(total) };

        return InvoiceEntity.CreateAsResult(newInvoice, invoiceRecord.Items);
    }

    public static Result<InvoiceEntity> CheckEntityRules(InvoiceEntity invoiceRecord)
    {
        var total = invoiceRecord.Items.Sum(item => item.Amount.Value);

        return invoiceRecord.Record.TotalAmount.Value == total
            ? Result<InvoiceEntity>.Success(invoiceRecord)
            : Result<InvoiceEntity>.Failure("The Invoice Total Amount is incorrect.");
    }

    public static InvoiceEntity Create()
        => new InvoiceEntity(DmoInvoice.Create(), Enumerable.Empty<DmoInvoiceItem>());

    public static InvoiceEntity Create(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => new InvoiceEntity(invoice, invoiceItems);

    public static Result<InvoiceEntity> CreateAsResult(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => Result<InvoiceEntity>.Create(Create(invoice, invoiceItems));
}
