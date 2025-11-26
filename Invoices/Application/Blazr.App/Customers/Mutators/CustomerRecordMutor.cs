/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class CustomerRecordMutor : IRecordMutor<DmoCustomer>
{
    public DmoCustomer BaseRecord { get; private init; }

    [TrackState] public string? Name { get; set; }

    public bool IsDirty => !this.Mutate().Equals(BaseRecord);

    public bool IsNew { get; private init; }

    public EditState State => this.IsNew
        ? EditState.New
        : IsDirty
            ? EditState.Dirty
            : EditState.Clean;

    private CustomerRecordMutor(DmoCustomer record)
    {
        this.BaseRecord = record;
        this.SetFields();
    }

    private void SetFields()
    {
        this.Name = this.BaseRecord.Name.Value;
    }

    public DmoCustomer Mutate() => this.BaseRecord with
    {
        Name = new(this.Name ?? "No Name Set")
    };

    public void Reset()
        => this.SetFields();

    public static CustomerRecordMutor Read(DmoCustomer record)
        => new CustomerRecordMutor(record);

    public static CustomerRecordMutor Create()
        => new CustomerRecordMutor(DmoCustomer.CreateNew()) { IsNew = true };
}
