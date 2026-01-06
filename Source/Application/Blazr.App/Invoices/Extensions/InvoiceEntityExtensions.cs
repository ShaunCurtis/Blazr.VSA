/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Invoices;

/// <summary>
/// Invoice Entity methods used exclusively within the Core Domain
/// By the Actions
/// </summary>
public static class InvoiceEntityExtensions
{
    extension(InvoiceEntity entity)
    {
        public Return<InvoiceEntity> ToReturnT => Return<InvoiceEntity>.Read(entity);

        public bool IsDirty(InvoiceEntity control) => !entity.Equals(control);

        public Return<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId id)
            => Return<DmoInvoiceItem>.Read(
                value: entity.InvoiceItems.SingleOrDefault(_item => _item.Id == id),
                errorMessage: "The record does not exist in the Invoice Items");

        public Return<InvoiceEntity> Mutate(DmoInvoice invoice)
            => InvoiceEntityFactory.Load(invoice, entity.InvoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);

        public Return<InvoiceEntity> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
            => InvoiceEntityFactory.Load(entity.InvoiceRecord, invoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);
    }
}
