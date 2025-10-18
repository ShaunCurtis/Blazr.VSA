/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

//public sealed record InvoiceEntity
//{
//    private readonly DroInvoice _baseRecord;
//    private readonly DroInvoice _record;

//    public DroInvoice AsRecord => _record;
//    public InvoiceId Id { get; private init; }
//    public DmoInvoice Invoice => _record.Record;
//    public IEnumerable<DmoInvoiceItem> InvoiceItems => _record.Items.AsEnumerable();
//    public bool IsDirty => _record.IsDirty(_baseRecord);

//    private InvoiceEntity(DroInvoice invoiceRecord)
//    {
//        this.Id = invoiceRecord.Record.Id;
//        _baseRecord = invoiceRecord;
//        _record = invoiceRecord;
//    }

//    private InvoiceEntity(DroInvoice invoice, DroInvoice baseInvoice)
//    {
//        this.Id = invoice.Record.Id;
//        _baseRecord = baseInvoice;
//        _record = invoice;
//    }

//    public Result<InvoiceEntity> ToResult
//        => Result<InvoiceEntity>.Create(this);

//    public static Result<InvoiceEntity> Create()
//    {
//        var entity = new InvoiceEntity(new DroInvoice(new DmoInvoice(), Enumerable.Empty<DmoInvoiceItem>()));
//        return Result<InvoiceEntity>.Success(entity);
//    }

//    public static Result<InvoiceEntity> Create(DroInvoice invoiceRecord)
//    {
//        return CheckEntityRules(invoiceRecord)
//            .ExecuteFunction<InvoiceEntity>(invoice => new InvoiceEntity(invoice));
//    }

//    public static Result<InvoiceEntity> Load(DroInvoice invoiceRecord, InvoiceEntity entity)
//    {
//        return ApplyEntityRules(invoiceRecord)
//            .ExecuteFunction<InvoiceEntity>(invoice => new InvoiceEntity(invoice, entity._baseRecord));
//    }

//    private static Result<DroInvoice> ApplyEntityRules(DroInvoice invoiceRecord)
//    {
//        var total = invoiceRecord.Items.Sum(item => item.Amount.Value);

//        if (invoiceRecord.Record.TotalAmount.Value == total)
//            return Result<DroInvoice>.Success(invoiceRecord);

//        var newInvoice = invoiceRecord.Record with { TotalAmount = new(total) };

//        return DroInvoice.CreateAsResult(newInvoice, invoiceRecord.Items);
//    }

//    private static Result<DroInvoice> CheckEntityRules(DroInvoice invoiceRecord)
//    {
//        var total = invoiceRecord.Items.Sum(item => item.Amount.Value);
        
//        return invoiceRecord.Record.TotalAmount.Value == total
//            ? Result<DroInvoice>.Success(invoiceRecord)
//            : Result<DroInvoice>.Failure("The Invoice Total Amount is incorrect.");
//    }
//}
