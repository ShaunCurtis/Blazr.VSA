/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Invoices;

internal static class InvoiceEntityExtensions
{
    extension(InvoiceEntity entity)
    {
        internal Return<InvoiceEntity> ToReturnT => Return<InvoiceEntity>.Read(entity);

        internal bool IsDirty(InvoiceEntity control) => !entity.Equals(control);

        internal Return<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId id)
            => Return<DmoInvoiceItem>.Read(
                value: entity.InvoiceItems.SingleOrDefault(_item => _item.Id == id),
                errorMessage: "The record does not exist in the Invoice Items");

        //internal Return<DmoInvoiceItem> GetInvoiceItem(DmoInvoiceItem item)
        //    => entity.GetInvoiceItem(item.Id);

        internal Return<InvoiceEntity> Mutate(DmoInvoice invoice)
            => InvoiceEntityFactory.Load(invoice, entity.InvoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);

        internal Return<InvoiceEntity> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
            => InvoiceEntityFactory.Load(entity.InvoiceRecord, invoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);
    }
}
