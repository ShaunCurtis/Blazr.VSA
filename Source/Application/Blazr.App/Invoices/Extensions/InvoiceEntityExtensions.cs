/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Invoices;

internal static class InvoiceEntityExtensions
{
    extension(InvoiceEntity @this)
    {
        internal Return<InvoiceEntity> ToReturnT => Return<InvoiceEntity>.Read(@this);

        internal bool IsDirty(InvoiceEntity control) => !@this.Equals(control);

        internal Return<InvoiceEntity> AddInvoiceItem(DmoInvoiceItem item)
            => @this.InvoiceItems.Any(_item => item.Id.Equals(_item.Id))
                  ? Return<InvoiceEntity>.Failure("The record already exists in the Invoice Items")
                  : @this.Mutate(@this.InvoiceItems.Add(item));

        internal Return<InvoiceEntity> DeleteInvoiceItem(DmoInvoiceItem record)
            => @this.GetInvoiceItem(record)
                .Bind(item => @this.Mutate(@this.InvoiceItems.Remove(item)));

        internal Return<InvoiceEntity> ReplaceInvoiceItem(DmoInvoiceItem record)
            => @this.GetInvoiceItem(record)
                .Bind(item => @this.Mutate(@this.InvoiceItems.Replace(item, record)));

        internal Return<InvoiceEntity> ReplaceInvoice(DmoInvoice record)
            => @this.Mutate(record);

        internal Return<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId id)
            => Return<DmoInvoiceItem>.Read(
                value: @this.InvoiceItems.SingleOrDefault(_item => _item.Id == id),
                errorMessage: "The record does not exist in the Invoice Items");

        private Return<DmoInvoiceItem> GetInvoiceItem(DmoInvoiceItem item)
            => @this.GetInvoiceItem(item.Id);

        private Return<InvoiceEntity> Mutate(DmoInvoice invoice)
            => InvoiceEntityFactory.Load(invoice, @this.InvoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);

        private Return<InvoiceEntity> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
            => InvoiceEntityFactory.Load(@this.InvoiceRecord, invoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);
    }
}
