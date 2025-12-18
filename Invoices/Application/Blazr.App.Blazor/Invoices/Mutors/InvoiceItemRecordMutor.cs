/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

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

    public static InvoiceItemRecordMutor Read(DmoInvoiceItem record)
        => new InvoiceItemRecordMutor(record);

    public static InvoiceItemRecordMutor Create(InvoiceId invoiceId)
        => new InvoiceItemRecordMutor(DmoInvoiceItem.CreateNew(invoiceId)) { IsNew = true };
}
