/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.Antimony.Mediator;

namespace Blazr.App.Invoice.Core;

public class CustomerEntityProvider : IEntityProvider<DmoCustomer, CustomerId>
{
    private readonly IMediatorBroker _mediator;

    public Func<CustomerId,  Task<Result<DmoCustomer>>> RecordRequest
        => (id) => _mediator.DispatchAsync(new CustomerRecordRequest(id));

    public Func<DmoCustomer, CommandState,  Task<Result<CustomerId>>> RecordCommand
        => (record, state) => _mediator.DispatchAsync(new CustomerCommandRequest(record, state));

    public Func<GridState<DmoCustomer>, Task<Result<ListItemsProvider<DmoCustomer>>>> ListRequest
        => (state) => _mediator.DispatchAsync(new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public CustomerEntityProvider(IMediatorBroker mediator)   
    {
        _mediator = mediator;
    }

    public CustomerId GetKey(object obj)
    {
        return obj switch
        {
            CustomerId id => id,
            DmoCustomer record => record.Id,
            Guid guid => new CustomerId(guid),
            _ => CustomerId.Default
        };
    }

    public bool TryGetKey(object obj, out CustomerId key)
    {
        key = GetKey(obj);
        return key != CustomerId.Default;
    }

    public DmoCustomer NewRecord
        => DefaultRecord;

    public static DmoCustomer DefaultRecord
        => new DmoCustomer { Id = CustomerId.Default };
}
