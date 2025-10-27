/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class CustomerRecordMutor
{
    public DmoCustomer BaseRecord { get; private init; }

    [TrackState] public string? Name { get; set; }

    public bool IsDirty => !this.AsRecord().Equals(BaseRecord);

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

    public DmoCustomer AsRecord() => this.BaseRecord with
    {
        Name = new(this.Name ?? "No Name Set")
    };

    public void Reset()
        => this.SetFields();

    public static CustomerRecordMutor Create(DmoCustomer record)
        => new CustomerRecordMutor(record);

    public static CustomerRecordMutor CreateNew()
        => new CustomerRecordMutor(DmoCustomer.CreateNewEntity()) { IsNew = true };
}
