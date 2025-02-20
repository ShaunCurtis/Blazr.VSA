/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class CustomerEditContext : BaseRecordEditContext<DmoCustomer, CustomerId>, IRecordEditContext<DmoCustomer>
{
    [TrackState] public string CustomerName { get; set; } = string.Empty;

    public override DmoCustomer AsRecord => this.BaseRecord with
    {
        CustomerName = this.CustomerName
    };

    public CustomerEditContext() : base() { }

    public CustomerEditContext(DmoCustomer record) : base(record) { }

    public override IDataResult Load(DmoCustomer record)
    {
        if (!this.BaseRecord.Id.IsDefault)
            return DataResult.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;
        this.CustomerName = record.CustomerName;
        return DataResult.Success();
    }
}
