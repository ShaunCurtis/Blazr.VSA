/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.UI;
using Blazr.Cadmium.Presentation;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;
using Blazr.Manganese;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public class CustomerUIConnector
   : IUIConnector<DmoCustomer, CustomerId>
{
    private readonly IMediatorBroker _mediator;

    public string SingleDisplayName { get; } = "Customer";
    public string PluralDisplayName { get; } = "Customers";
    public Type? EditForm { get; } = typeof(CustomerEditForm);
    public Type? ViewForm { get; } = typeof(CustomerViewForm);
    public string Url { get; } = "/Customer";

    public CustomerUIConnector(IMediatorBroker mediator)
    {
        _mediator = mediator;
    }

    public Func<CustomerId, Task<Return<DmoCustomer>>> RecordRequestAsync
        => id => id.IsDefault 
            ? NewRecordRequestAsync(id) 
            : ExistingRecordRequestAsync(id);

    public Func<DmoCustomer, EditState, Task<Return<CustomerId>>> RecordCommandAsync
        => (record, state) => _mediator.DispatchAsync(new CustomerCommandRequest(record, state));

    public Task<Return<GridItemsProviderResult<DmoCustomer>>> GetItemsAsync(GridState<DmoCustomer> state)
        => state.ToReturnT
            .Bind(CustomerListRequest.FromGridState)
            .BindAsync(request => _mediator.DispatchAsync(request))
            .MapAsync(itemsProvider => itemsProvider.ToGridItemsProviderResult());

    public IRecordMutor<DmoCustomer> GetRecordMutor(DmoCustomer customer)
        => CustomerRecordMutor.Load(customer);

    private Func<CustomerId, Task<Return<DmoCustomer>>> ExistingRecordRequestAsync
        => id => _mediator.DispatchAsync(new CustomerRecordRequest(id));

    private Func<CustomerId, Task<Return<DmoCustomer>>> NewRecordRequestAsync
        => id => Task.FromResult(ReturnT.Read(new DmoCustomer { Id = CustomerId.NewId() }));
}
