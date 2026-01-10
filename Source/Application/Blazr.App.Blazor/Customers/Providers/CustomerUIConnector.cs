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

    public Func<CustomerId, Task<Result<DmoCustomer>>> RecordRequestAsync
        => id => id.IsNew 
            ? Task.FromResult(ResultT.Successful(new DmoCustomer { Id = CustomerId.NewId }))
            : _mediator.DispatchAsync(new CustomerRecordRequest(id));

    public Func<DmoCustomer, RecordState, Task<Result<CustomerId>>> RecordCommandAsync
        => (record, state) => _mediator.DispatchAsync(new CustomerCommandRequest(record, state));

    public Task<Result<GridItemsProviderResult<DmoCustomer>>> GetItemsAsync(GridState<DmoCustomer> state)
        => state.ToResultT
            .Bind(CustomerListRequest.FromGridState)
            .BindAsync(request => _mediator.DispatchAsync(request))
            .MapAsync(itemsProvider => itemsProvider.ToGridItemsProviderResult());

    public IRecordMutor<DmoCustomer> GetRecordMutor(DmoCustomer customer)
        => CustomerRecordMutor.Load(customer);
}
