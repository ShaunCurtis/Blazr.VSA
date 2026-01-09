/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public sealed class CustomerRecordMutor : RecordMutor<DmoCustomer>, IRecordMutor<DmoCustomer>
{
    [TrackState] public string? Name { get; set; }
    public override bool IsNew => BaseRecord.Id.IsNew;

    private CustomerRecordMutor(DmoCustomer record)
    {
        this.BaseRecord = record;
        this.SetFields();
    }

    private void SetFields()
    {
        this.Name = this.BaseRecord.Name.Value;
    }

    public override DmoCustomer Record => this.BaseRecord with
    {
        Name = new(this.Name ?? "No Name Set")
    };

    public void Reset()
        => this.SetFields();

    public static CustomerRecordMutor Load(DmoCustomer record)
        => new CustomerRecordMutor(record);

    public static CustomerRecordMutor NewMutor()
        => new CustomerRecordMutor(DmoCustomer.NewCustomer());
}
