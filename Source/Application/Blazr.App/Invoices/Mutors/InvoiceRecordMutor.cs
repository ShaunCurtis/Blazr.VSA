/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class InvoiceRecordMutor : RecordMutor<DmoInvoice>, IRecordMutor<DmoInvoice>
{
    [TrackState] public DateOnly Date { get; set; }
    [TrackState] public Guid CustomerId { get; set; }
    public FkoCustomer Customer { get; set; } = FkoCustomer.Default;

    private InvoiceRecordMutor(DmoInvoice record)
    {
        this.BaseRecord = record;
        this.SetFields();
    }

    private void SetFields()
    {
        this.Date = this.BaseRecord.Date.Value;
        this.CustomerId = this.BaseRecord.Customer.Id.Value;
        this.Customer = this.BaseRecord.Customer;
    }

    public override DmoInvoice Record => this.BaseRecord with
    {
        Date = new Date(this.Date),
         Customer = this.Customer
    };

    public void Reset()
        => this.SetFields();

    public Func<InvoiceEntity, Return<InvoiceEntity>> Dispatcher =>
        entity => (this.IsNew, this.IsDirty) switch
        {
            (false, true) => UpdateInvoiceAction.Create(this.Record).Dispatcher(entity),
            _ => ReturnT.Read(entity),
        };

    public static InvoiceRecordMutor Read(DmoInvoice record)
        => new InvoiceRecordMutor(record);

    public static InvoiceRecordMutor Create()
        => new InvoiceRecordMutor(DmoInvoice.CreateNew()) { IsNew = true };
}
