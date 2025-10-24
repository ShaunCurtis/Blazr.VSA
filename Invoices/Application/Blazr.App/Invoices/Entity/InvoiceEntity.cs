/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using System.Collections.Immutable;

namespace Blazr.App;

public InvoiceRecord InvoiceEntity
{
    public DmoInvoice InvoiceRecord { get; private init; }
    public ImmutableList<DmoInvoiceItem> Items { get; private init; }

    private InvoiceEntity(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
    {
        this.InvoiceRecord = invoice;
        this.Items = invoiceItems.ToImmutableList();
    }

    public bool IsDirty(InvoiceEntity control)
    {
        if (control.InvoiceRecord is null || !control.InvoiceRecord.Equals(this.InvoiceRecord))
            return true;
        if (control.Items.Count != this.Items.Count)
            return true;
        if (control.Items.Except(this.Items).Count() != 0)
            return true;

        return false;
    }

    public Result<InvoiceEntity> ToResult => Result<InvoiceEntity>.Create(this);

    public static Result<InvoiceEntity> CheckEntityRules(InvoiceEntity entity)
    {
        var total = entity.Items.Sum(item => item.Amount.Value);

        return entity.InvoiceRecord.TotalAmount.Value == total
            ? Result<InvoiceEntity>.Success(entity)
            : Result<InvoiceEntity>.Failure("The Invoice Total Amount is incorrect.");
    }

    public static Result<InvoiceEntity> ApplyEntityRules(InvoiceEntity entity)
    {
        var total = entity.Items.Sum(item => item.Amount.Value);

        if (entity.InvoiceRecord.TotalAmount.Value == total)
            return Result<InvoiceEntity>.Success(entity);

        var newInvoice = entity.InvoiceRecord with { TotalAmount = new(total) };

        return InvoiceEntity.Create(newInvoice, entity.Items);
    }

    public static InvoiceEntity CreateNew
        => new InvoiceEntity(DmoInvoice.Create(), Enumerable.Empty<DmoInvoiceItem>());

    public static Result<InvoiceEntity> Create(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => CheckEntityRules(new InvoiceEntity(invoice, invoiceItems));

    public static Result<InvoiceEntity> CreateWithRules(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => ApplyEntityRules(new InvoiceEntity(invoice, invoiceItems));
}
