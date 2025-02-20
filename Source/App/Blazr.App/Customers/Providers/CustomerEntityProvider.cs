/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class CustomerEntityProvider : IEntityProvider<DmoCustomer, CustomerId>
{
    public Func<IMediator, CustomerId,  Task<Result<DmoCustomer>>> RecordRequest
        => (broker, id) => broker.Send(new CustomerRecordRequest(id));

    public Func<IMediator, DmoCustomer, CommandState,  Task<Result<CustomerId>>> RecordCommand
        => (broker, record, state) => broker.Send(new CustomerCommandRequest(record, state));

    public CustomerId GetKey(object key)
    {
        return key switch
        {
            CustomerId id => id,
            Guid guid => new CustomerId(guid),
            _ => CustomerId.Default
        };
    }

    public CustomerId GetKey(DmoCustomer record)
    {
        return record.Id;
    }

    public DmoCustomer NewRecord
        => DefaultRecord;

    public static DmoCustomer DefaultRecord
        => new DmoCustomer { Id = CustomerId.Default };
}
