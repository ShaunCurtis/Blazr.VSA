/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.App.UI;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using Blazr.Manganese;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public class CustomerUIConnector
   : UIConnector<DmoCustomer>,
    IUIConnector<DmoCustomer, CustomerId>
{
    private readonly IMediatorBroker _mediator;
    private readonly IServiceProvider _serviceProvider;

    public string SingleDisplayName { get; } = "Customer";
    public string PluralDisplayName { get; } = "Customers";
    public Type? EditForm { get; } = typeof(CustomerEditForm);
    public Type? ViewForm { get; } = typeof(CustomerViewForm);
    public string Url { get; } = "/Customer";

    public CustomerUIConnector(IMediatorBroker mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public Func<CustomerId, Task<Result<DmoCustomer>>> RecordRequestAsync
        => id => id.IsDefault ? NewRecordRequestAsync(id) : ExistingRecordRequestAsync(id);

    public Func<StateRecord<DmoCustomer>, Task<Result<CustomerId>>> RecordCommandAsync
        => record => _mediator.DispatchAsync(new CustomerCommandRequest(record));

    public Func<GridState<DmoCustomer>, Task<Result<ListItemsProvider<DmoCustomer>>>> GridItemsRequestAsync
        => state => _mediator.DispatchAsync(new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public Task<Result<GridItemsProviderResult<DmoCustomer>>> GetItemsAsync(GridState<DmoCustomer> state)
        => CustomerListRequest.Create(state)
            .ExecuteTransformAsync((request) => _mediator.DispatchAsync(request))
            .ExecuteFunctionAsync((itemsProvider) => GridItemsProviderResult
                .From<DmoCustomer>(itemsProvider.Items.ToList(), itemsProvider.TotalCount));

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

    public IRecordMutor<DmoCustomer> GetRecordMutor(DmoCustomer customer)
        => CustomerRecordMutor.Create(customer);

    public IRecordMutor<DmoCustomer> GetNewRecordMutor()
        => CustomerRecordMutor.CreateNew();

    private Func<CustomerId, Task<Result<DmoCustomer>>> ExistingRecordRequestAsync
        => id => _mediator.DispatchAsync(new CustomerRecordRequest(id));

    private Func<CustomerId, Task<Result<DmoCustomer>>> NewRecordRequestAsync
        => id => Task.FromResult(Result<DmoCustomer>.Create(new DmoCustomer { Id = CustomerId.Default }));
}
