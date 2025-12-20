/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Invoices;

public static class InvoiceEntityFactory
{
    internal static InvoiceEntity Create() =>
        InvoiceEntity.Read(DmoInvoice.CreateNew(), Enumerable.Empty<DmoInvoiceItem>());

    internal static InvoiceEntity Create(DmoInvoice invoice) =>
        InvoiceEntity.Read(invoice, Enumerable.Empty<DmoInvoiceItem>());

    /// <summary>
    /// Attempts to load an invoice entity from the specified invoice and its associated items, validating business rules in
    /// the process.
    /// </summary>
    /// <param name="invoice">The invoice data to load. Cannot be null.</param>
    /// <param name="invoiceItems">The collection of items associated with the invoice. Cannot be null.</param>
    /// <returns>A <see cref="Return{InvoiceEntity}"/> that contains the loaded invoice entity if validation succeeds; otherwise,
    /// contains validation errors.</returns>
    public static Return<InvoiceEntity> TryLoad(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
        CheckEntityRules(InvoiceEntity.Read(invoice, invoiceItems));

    /// <summary>
    /// Creates an invoice entity from the specified invoice and its items, applying validation and business rules.
    /// </summary>
    /// <param name="invoice">The invoice data to be loaded into the entity. Cannot be null.</param>
    /// <param name="invoiceItems">The collection of items associated with the invoice. Cannot be null.</param>
    /// <returns>A result containing the created invoice entity if all rules are satisfied; otherwise, a result indicating validation
    /// errors.</returns>
    public static Return<InvoiceEntity> Load(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
        ApplyEntityRules(InvoiceEntity.Read(invoice, invoiceItems)).ToReturnT;

    internal static Return<InvoiceEntity> CheckEntityRules(InvoiceEntity entity)
        => entity.InvoiceItems.Sum(item => item.Amount.Value) == entity.InvoiceRecord.TotalAmount.Value
            ? Return<InvoiceEntity>.Success(entity)
            : Return<InvoiceEntity>.Failure("The Invoice Total Amount is incorrect.");

    internal static InvoiceEntity ApplyEntityRules(InvoiceEntity entity)
        => InvoiceEntity.Read(
            invoice: entity.InvoiceRecord with { TotalAmount = new(entity.InvoiceItems.Sum(item => item.Amount.Value)) },
            invoiceItems: entity.InvoiceItems);
}
