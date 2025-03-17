/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Presentation;

namespace Blazr.App.Core;

public class CustomerEntityProvider : IEntityProvider<DmoCustomer, CustomerId>
{
    private readonly IMediator _mediator;

    public Func<CustomerId,  Task<Result<DmoCustomer>>> RecordRequest
        => (id) => _mediator.Send(new CustomerRecordRequest(id));

    public Func<DmoCustomer, CommandState,  Task<Result<CustomerId>>> RecordCommand
        => (record, state) => _mediator.Send(new CustomerCommandRequest(record, state));

    public Func<GridState<DmoCustomer>, Task<Result<ListItemsProvider<DmoCustomer>>>> ListRequest
        => (state) => _mediator.Send(new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public CustomerEntityProvider(IMediator mediator)   
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
