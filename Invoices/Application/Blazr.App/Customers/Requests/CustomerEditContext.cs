/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;

namespace Blazr.App.Core;

public sealed class CustomerEditContext : BaseRecordEditContext<DmoCustomer, CustomerId>, IRecordEditContext<DmoCustomer>
{
    [TrackState] public string? Name { get; set; }

    public CustomerEditContext() : base() { }

    public CustomerEditContext(DmoCustomer record) : base(record) { }

    public override DmoCustomer AsRecord =>
    this.BaseRecord with
    {
        Name = new(this.Name ?? "No Name Set")
    };

    public override Result<DmoCustomer> ToResult 
        => Result<DmoCustomer>.Create(this.BaseRecord with
            {
                Name = new(this.Name ?? "No Name Set")
            });

    public override Result Load(DmoCustomer record)
    {
        if (!this.BaseRecord.Id.IsDefault)
            return Result.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;

        this.Name = record.Name.Value;

        return  Result.Success();
    }
}
