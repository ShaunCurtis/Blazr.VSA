/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public class CustomerEntityProvider
   : EntityProvider<DmoCustomer>,
    IEntityProvider<DmoCustomer, CustomerId>
{
    public CustomerEntityProvider(IMediatorBroker mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public Task<Result<GridItemsProviderResult<DmoCustomer>>> GetItemsAsync(GridState<DmoCustomer> state)
        => CustomerListRequest.Create(state)
            .DispatchAsync((request) => _mediator.DispatchAsync(request))
            .ExecuteFunctionAsync((itemsProvider) => GridItemsProviderResult
                .From<DmoCustomer>(itemsProvider.Items.ToList(), itemsProvider.TotalCount));

    public Func<CustomerId, Task<Result<DmoCustomer>>> RecordRequestAsync
        => (id) => id.IsDefault ? NewRecordRequestAsync(id) : ExistingRecordRequestAsync(id);

    public Func<StateRecord<DmoCustomer>, Task<Result<CustomerId>>> RecordCommandAsync
        => (record) => _mediator.DispatchAsync(new CustomerCommandRequest(record));

    public Func<GridState<DmoCustomer>, Task<Result<ListItemsProvider<DmoCustomer>>>> GridItemsRequestAsync
        => (state) => _mediator.DispatchAsync(new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public Func<CustomerListRequest, Task<Result<ListItemsProvider<DmoCustomer>>>> ListItemsRequestAsync
        => (request) => _mediator.DispatchAsync(request);

    public Result<CustomerId> GetKey(object? obj)
        => obj switch
        {
            CustomerId id => Result<CustomerId>.Create(id),
            DmoCustomer record => Result<CustomerId>.Create(record.Id),
            Guid guid => Result<CustomerId>.Create(new(guid)),
            _ => Result<CustomerId>.Failure($"Could not convert the provided key - {obj?.ToString()}")
        };

    public DmoCustomer NewRecord
        => new DmoCustomer { Id = CustomerId.Default };

    private readonly IMediatorBroker _mediator;
    private readonly IServiceProvider _serviceProvider;

    private Func<CustomerId, Task<Result<DmoCustomer>>> ExistingRecordRequestAsync
        => (id) => _mediator.DispatchAsync(new CustomerRecordRequest(id));

    private Func<CustomerId, Task<Result<DmoCustomer>>> NewRecordRequestAsync
        => (id) => Task.FromResult(Result<DmoCustomer>.Create(new DmoCustomer { Id = CustomerId.Default }));
}
