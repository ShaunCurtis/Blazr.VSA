/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public sealed class InvoiceRecordMutor : RecordMutor<DmoInvoice>, IRecordMutor<DmoInvoice>
{
    [TrackState] public DateOnly Date { get; set; }
    [TrackState] public Guid CustomerId { get; set; }
    public FkoCustomer Customer
    {
        get => field ?? FkoCustomer.Default;
        set
        {
            field = value;
            if (value is not null)
                this.CustomerId = value.Id.Value;
        }
    }

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

    public override bool IsNew => BaseRecord.Id.IsNew;

    public Func<InvoiceEntity, Return<InvoiceEntity>> Dispatcher
        => entity => this.IsDirty || this.IsNew
            ? UpdateInvoiceAction.Create(this.Record).Dispatcher(entity)
            : ReturnT.Read(entity);

    public static InvoiceRecordMutor Load(DmoInvoice record)
        => new InvoiceRecordMutor(record);

    public static InvoiceRecordMutor NewMutor()
        => new InvoiceRecordMutor(DmoInvoice.CreateNew());
}
