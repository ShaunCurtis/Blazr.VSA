/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;

namespace Blazr.App.Core;

public sealed class CustomerRecordMutor
{
    public DmoCustomer BaseRecord { get; private init; }

    [TrackState] public string? Name { get; set; }

    public bool IsDirty => !this.AsRecord.Equals(BaseRecord);

    private CustomerRecordMutor(DmoCustomer record)
    {
        this.BaseRecord = record;
        this.SetFields();
    }

    private void SetFields()
    {
        this.Name = this.BaseRecord.Name.Value;
    }

    public DmoCustomer AsRecord => this.BaseRecord with
    {
        Name = new(this.Name ?? "No Name Set")
    };

    public void Reset()
        => this.SetFields();

    public static CustomerRecordMutor Load(DmoCustomer record)
        => new CustomerRecordMutor(record);
}
