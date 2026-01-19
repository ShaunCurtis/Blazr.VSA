/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public sealed class InvoiceItemRecordMutor : RecordMutor<DmoInvoiceItem> ,IRecordMutor<DmoInvoiceItem>
{
    [TrackState] public string Description { get; set; } = string.Empty;
    [TrackState] public decimal Amount { get; set; }

    private InvoiceItemRecordMutor(DmoInvoiceItem record)
    {
        this.BaseRecord = record;
        this.SetFields();
    }

    private void SetFields()
    {
        this.Description = this.BaseRecord.Description.Value;
        this.Amount = this.BaseRecord.Amount.Value;
    }

    public override DmoInvoiceItem Record => this.BaseRecord with
    {
        Description = new(this.Description),
        Amount = new(this.Amount)
    };

    public void Reset()
        => this.SetFields();

    public Func<InvoiceEntity, Result<InvoiceEntity>> Dispatcher =>
        entity => this.IsDirty
            ? SaveInvoiceItemAction.Create(this.Record).Dispatcher(entity)
            : ResultT.Successful(entity);

    public override bool IsNew => BaseRecord.Id.IsNew;

    public static InvoiceItemRecordMutor Load(DmoInvoiceItem record)
        => new InvoiceItemRecordMutor(record);

    public static InvoiceItemRecordMutor NewMutor(InvoiceId invoiceId)
        => new InvoiceItemRecordMutor(DmoInvoiceItem.CreateNew(invoiceId));
}
