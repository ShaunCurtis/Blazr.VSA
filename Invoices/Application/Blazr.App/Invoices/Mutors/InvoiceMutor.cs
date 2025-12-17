/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App;

public sealed record InvoiceMutor
{
    private readonly InvoiceEntity BaseEntity;
    
    public InvoiceEntity CurrentEntity { get; private init; }
    public bool IsNew { get; private init; }

    public InvoiceId Id => BaseEntity.InvoiceRecord.Id;

    public Return<InvoiceMutor> ToResult => Return<InvoiceMutor>.Read(this);

    public bool IsDirty => this.CurrentEntity.IsDirty(BaseEntity);

    public EditState State => this.IsNew 
        ? EditState.New 
        : IsDirty 
            ? EditState.Dirty 
            : EditState.Clean;

    private InvoiceMutor(InvoiceEntity baseEntity)
    {
        this.BaseEntity = baseEntity;
        this.CurrentEntity = baseEntity;
    }

    private InvoiceMutor(InvoiceEntity mutatedEntity, InvoiceEntity baseEntity)
    {
        this.BaseEntity = baseEntity;
        this.CurrentEntity = mutatedEntity;
    }

    /// <summary>
    /// Creates and returns a new InvoiceMutor using the supplied invoice and invoice items
    /// and the base invoice entity from the current mutor
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="invoiceItems"></param>
    /// <returns></returns>
    public Return<InvoiceMutor> Mutate(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => InvoiceEntity.CreateWithEntityRulesApplied(invoice, invoiceItems)
            .Bind(entity => InvoiceMutor.CreateMutation(entity, this.BaseEntity).ToResult);

    /// <summary>
    /// Creates and returns a new InvoiceMutor using the supplied invoice
    /// and the invoice items and base invoice entity from the current mutor
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="invoiceItems"></param>
    /// <returns></returns>
    public Return<InvoiceMutor> Mutate(DmoInvoice invoice)
        => InvoiceEntity.CreateWithEntityRulesApplied(invoice, this.CurrentEntity.InvoiceItems)
            .Bind(entity => InvoiceMutor.CreateMutation(entity, this.BaseEntity).ToResult);

    /// <summary>
    /// Creates and returns a new InvoiceMutor using the supplied invoice items
    /// and the invoice and base invoice entity from the current mutor
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="invoiceItems"></param>
    /// <returns></returns>
    public Return<InvoiceMutor> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
        => InvoiceEntity.CreateWithEntityRulesApplied(this.CurrentEntity.InvoiceRecord, invoiceItems)
            .Bind(entity => InvoiceMutor.CreateMutation(entity, this.BaseEntity).ToResult);

    /// <summary>
    /// Contructor to create a New Invoice i.e. an Invoice with default values
    /// that will be treated as an Create/Add by the data pipeline
    /// </summary>
    /// <returns></returns>
    public static InvoiceMutor CreateNew()
        => new InvoiceMutor(InvoiceEntity.CreateNewEntity()) { IsNew = true };

    /// <summary>
    /// Constructor to create a InvoiceMutor from an existing Invoice
    /// Do not use it to create a brand new invoice - Use CreateNew
    /// </summary>
    /// <param name="baseEntity"></param>
    /// <returns></returns>
    public static InvoiceMutor Create(InvoiceEntity baseEntity)
        => new InvoiceMutor(baseEntity);

    private static InvoiceMutor CreateMutation(InvoiceEntity mutatedEntity, InvoiceEntity baseEntity)
        => new InvoiceMutor(mutatedEntity, baseEntity);
}
