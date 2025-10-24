/// ============================================================
/// Author: Shaun Curtis, Cold Elm Codersh {}
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App;

public InvoiceRecord InvoiceMutor
{
    public InvoiceEntity BaseEntity { get; private init; }
    public InvoiceEntity MutatedEntity { get; private init; }

    public InvoiceId Id => BaseEntity.InvoiceRecord.Id;

    private InvoiceMutor(InvoiceEntity baseEntity)
    {
        this.BaseEntity = baseEntity;
        this.MutatedEntity = baseEntity;
    }
    private InvoiceMutor(InvoiceEntity mutatedEntity, InvoiceEntity baseEntity)
    {
        this.BaseEntity = baseEntity;
        this.MutatedEntity = mutatedEntity;
    }

    public Result<InvoiceMutor> ToResult => Result<InvoiceMutor>.Create(this);

    public bool IsDirty => this.MutatedEntity.IsDirty(BaseEntity);

    public Result<InvoiceMutor> Mutate(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => InvoiceEntity.CreateWithRules(invoice, invoiceItems)
            .ExecuteTransform(entity => InvoiceMutor.Create(entity, this.BaseEntity).ToResult);

    public Result<InvoiceMutor> Mutate(DmoInvoice invoice)
        => InvoiceEntity.CreateWithRules(invoice, this.MutatedEntity.Items)
            .ExecuteTransform(entity => InvoiceMutor.Create(entity, this.BaseEntity).ToResult);

    public Result<InvoiceMutor> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
        => InvoiceEntity.CreateWithRules(this.MutatedEntity.InvoiceRecord, invoiceItems)
            .ExecuteTransform(entity => InvoiceMutor.Create(entity, this.BaseEntity).ToResult);

    public static InvoiceMutor CreateNew()
        => Create(InvoiceEntity.CreateNew);

    public static InvoiceMutor Create(InvoiceEntity baseEntity)
        => new InvoiceMutor(baseEntity);

    public static InvoiceMutor Create(InvoiceEntity mutatedEntity, InvoiceEntity baseEntity)
        => new InvoiceMutor(mutatedEntity, baseEntity);
}
