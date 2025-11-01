/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class InvoiceRecordMutor : IRecordMutor<DmoInvoice>
{
    public DmoInvoice BaseRecord { get; private init; }

    [TrackState] public DateOnly Date { get; set; }
    [TrackState] public Guid CustomerId { get; set; }
    public FkoCustomer Customer { get; set; } = FkoCustomer.Default;

    public bool IsDirty => !this.ToRecord().Equals(BaseRecord);

    public bool IsNew { get; private init; }

    public EditState State => this.IsNew
        ? EditState.New
        : IsDirty
            ? EditState.Dirty
            : EditState.Clean;

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

    public DmoInvoice ToRecord() => this.BaseRecord with
    {
        Date = new Date(this.Date),
         Customer = this.Customer
    };

    public Result<DmoInvoice> ToResult()
        => Result<DmoInvoice>.Success(this.ToRecord());

    public void Reset()
        => this.SetFields();

    public static InvoiceRecordMutor Create(DmoInvoice record)
        => new InvoiceRecordMutor(record);

    public static InvoiceRecordMutor CreateNewEntity()
        => new InvoiceRecordMutor(DmoInvoice.CreateNew()) { IsNew = true };
}
