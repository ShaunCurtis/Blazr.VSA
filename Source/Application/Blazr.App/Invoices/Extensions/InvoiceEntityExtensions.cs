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
    extension(InvoiceEntity @this)
    {
        public Return<InvoiceEntity> ToReturnT => Return<InvoiceEntity>.Read(@this);

        public InvoiceEntity Map(Func<InvoiceEntity, InvoiceEntity> func)
            => func.Invoke(@this);

        public bool IsDirty(InvoiceEntity control) => !@this.Equals(control);

        public Return<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId id)
            => Return<DmoInvoiceItem>.Read(
                value: @this.InvoiceItems.SingleOrDefault(_item => _item.Id == id),
                errorMessage: "The record does not exist in the Invoice Items");

        public InvoiceEntity Mutate(DmoInvoice invoice)
            => InvoiceEntityFactory.Load(invoice, @this.InvoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);

        public InvoiceEntity Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
            => InvoiceEntityFactory.Load(@this.InvoiceRecord, invoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);
    }
}
