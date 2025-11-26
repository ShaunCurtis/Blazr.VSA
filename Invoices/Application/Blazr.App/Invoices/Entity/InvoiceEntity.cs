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

    public Bool<InvoiceEntity> ToBoolT()
        => Bool<InvoiceEntity>.Read(this);

    public static InvoiceEntity CreateNewEntity() =>
        new InvoiceEntity(DmoInvoice.CreateNew(), Enumerable.Empty<DmoInvoiceItem>());
    public static InvoiceEntity CreateNewEntity(DmoInvoice invoice) =>
        new InvoiceEntity(invoice, Enumerable.Empty<DmoInvoiceItem>());

    public static Bool<InvoiceEntity> CreateWithRulesValidation(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
        CheckEntityRules(new InvoiceEntity(invoice, invoiceItems));

    public Bool<InvoiceEntity> CreateWithRulesValidation(DmoInvoice invoice) =>
        ApplyEntityRules(new InvoiceEntity(invoice, this.InvoiceItems)).ToBoolT();

    public Bool<InvoiceEntity> CreateWithRulesValidation(IEnumerable<DmoInvoiceItem> invoiceItems) =>
        ApplyEntityRules(new InvoiceEntity(this.InvoiceRecord, invoiceItems)).ToBoolT();

    public static Bool<InvoiceEntity> CreateWithEntityRulesApplied(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
        ApplyEntityRules(new InvoiceEntity(invoice, invoiceItems)).ToBoolT();

    public Bool<InvoiceEntity> CreateWithEntityRulesApplied(DmoInvoice invoice) =>
        ApplyEntityRules(new InvoiceEntity(invoice, this.InvoiceItems)).ToBoolT();

    public Bool<InvoiceEntity> CreateWithEntityRulesApplied(IEnumerable<DmoInvoiceItem> invoiceItems) =>
        ApplyEntityRules(new InvoiceEntity(this.InvoiceRecord, invoiceItems)).ToBoolT();

    private static InvoiceEntity Create(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
        new InvoiceEntity(invoice, invoiceItems);

    public static Bool<InvoiceEntity> CheckEntityRules(InvoiceEntity entity)
        => entity.InvoiceItems.Sum(item => item.Amount.Value) == entity.InvoiceRecord.TotalAmount.Value
            ? Bool<InvoiceEntity>.Success(entity)
            : Bool<InvoiceEntity>.Failure("The Invoice Total Amount is incorrect.");

    private static InvoiceEntity ApplyEntityRules(InvoiceEntity entity)
        => InvoiceEntity.Create(
            invoice: entity.InvoiceRecord with { TotalAmount = new(entity.InvoiceItems.Sum(item => item.Amount.Value)) },
            invoiceItems: entity.InvoiceItems);
}
