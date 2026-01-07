/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Collections.Immutable;

namespace Blazr.App.Core;

/// <summary>
/// Invoice Entity for the InvoiceMutor
/// </summary>
public sealed record InvoiceEntity : IEquatable<InvoiceEntity>
{
    public DmoInvoice InvoiceRecord { get; private init; }
    public ImmutableList<DmoInvoiceItem> InvoiceItems { get; private init; }

    private InvoiceEntity(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceInvoiceItems)
    {
        this.InvoiceRecord = invoice;
        this.InvoiceItems = invoiceInvoiceItems.ToImmutableList();
    }

    public static InvoiceEntity Load(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) 
        => new InvoiceEntity(invoice, invoiceItems);

    public bool Equals(InvoiceEntity? other)
        => other is not null 
            && this.InvoiceItems.OrderBy(e => e.Id.Value).SequenceEqual(other.InvoiceItems.OrderBy(e => e.Id.Value))
            && this.InvoiceRecord.Equals(other.InvoiceRecord);

    public override int GetHashCode()
        => base.GetHashCode();
}
