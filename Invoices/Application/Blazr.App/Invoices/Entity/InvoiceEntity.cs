/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using System.Collections.Immutable;

namespace Blazr.App;

public sealed record InvoiceEntity
{
    public DmoInvoice InvoiceRecord { get; private init; }
    public ImmutableList<DmoInvoiceItem> InvoiceItems { get; private init; }

    private InvoiceEntity(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceInvoiceItems)
    {
        this.InvoiceRecord = invoice;
        this.InvoiceItems = invoiceInvoiceItems.ToImmutableList();
    }

    public bool IsDirty(InvoiceEntity control)
        => !this.Equals(control);

    public Result<InvoiceEntity> ToResult()
        => Result<InvoiceEntity>.Create(this);

    public static InvoiceEntity CreateNewEntity() =>
        new InvoiceEntity(DmoInvoice.Create(), Enumerable.Empty<DmoInvoiceItem>());

    public static Result<InvoiceEntity> CreateWithRulesValidation(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
        CheckEntityRules(new InvoiceEntity(invoice, invoiceItems));

    public static Result<InvoiceEntity> CreateWithEntityRulesApplied(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
        ApplyEntityRules(new InvoiceEntity(invoice, invoiceItems));

    private static InvoiceEntity Create(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
        new InvoiceEntity(invoice, invoiceItems);

    public static Result<InvoiceEntity> CheckEntityRules(InvoiceEntity entity)
        => entity.InvoiceItems.Sum(item => item.Amount.Value) == entity.InvoiceRecord.TotalAmount.Value
            ? Result<InvoiceEntity>.Success(entity)
            : Result<InvoiceEntity>.Failure("The Invoice Total Amount is incorrect.");

    public static Result<InvoiceEntity> ApplyEntityRules(InvoiceEntity entity)
        => InvoiceEntity.Create(
            invoice: entity.InvoiceRecord with { TotalAmount = new(entity.InvoiceItems.Sum(item => item.Amount.Value)) },
            invoiceItems: entity.InvoiceItems)
        .ToResult();
}
