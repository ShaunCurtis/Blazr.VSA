/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Collections.Immutable;

namespace Blazr.App.Core;

public sealed record InvoiceEntity
{
    public DmoInvoice InvoiceRecord { get; private init; }
    public ImmutableList<DmoInvoiceItem> InvoiceItems { get; private init; }

    private InvoiceEntity(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceInvoiceItems)
    {
        this.InvoiceRecord = invoice;
        this.InvoiceItems = invoiceInvoiceItems.ToImmutableList();
    }

    internal static InvoiceEntity Load(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) 
        => new InvoiceEntity(invoice, invoiceItems);


    public bool Equals(InvoiceEntity? other)
    { 
        if (other == null) 
            return false;

        return  this.InvoiceItems.OrderBy(e => e).SequenceEqual(other.InvoiceItems.OrderBy(e => e))
            &&  this.InvoiceRecord.Equals(other.InvoiceRecord);
    }

    public override int GetHashCode()
        => base.GetHashCode();
}
