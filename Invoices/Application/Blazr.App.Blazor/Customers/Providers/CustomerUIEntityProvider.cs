/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.Presentation;
using Blazr.Cadmium.QuickGrid;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.UI;

public sealed record CustomerUIEntityProvider : IUIEntityProvider<DmoCustomer, CustomerId>
{
    private readonly IServiceProvider _serviceProvider;

    public string SingleDisplayName { get; } = "Customer";
    public string PluralDisplayName { get; } = "Customers";
    public Type? EditForm { get; } = typeof(CustomerEditForm);
    public Type? ViewForm { get; } = typeof( CustomerViewForm);
    public string Url { get; } = "/Customer";

    public CustomerUIEntityProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IReadUIBroker<DmoCustomer, CustomerId>> GetReadUIBrokerAsync(CustomerId id)
    {
        var presenter = ActivatorUtilities.CreateInstance<ReadUIBroker<DmoCustomer, CustomerId>>(_serviceProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }

    public async ValueTask<IEditUIBroker<DmoCustomer,TRecordMutor, CustomerId>> GetEditUIBrokerAsync<TRecordMutor>(CustomerId id)
        where TRecordMutor : IRecordMutor<DmoCustomer>
    {
        var presenter = ActivatorUtilities.CreateInstance<EditUIBroker<DmoCustomer, TRecordMutor, CustomerId>>(_serviceProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }

    public async ValueTask<IGridUIBroker<DmoCustomer>> GetGridUIBrokerAsync(Guid contextId)
    {
        var broker = await GetGridUIBrokerAsync();
        broker.SetContext(contextId);
        return broker;
    }

    public async ValueTask<IGridUIBroker<DmoCustomer>> GetGridUIBrokerAsync(Guid contextId, UpdateGridRequest<DmoCustomer> resetGridRequest)
    {
        var broker = await GetGridUIBrokerAsync();
        broker.SetContext(contextId, resetGridRequest);
        return broker;
    }

    private ValueTask<IGridUIBroker<DmoCustomer>> GetGridUIBrokerAsync()
    {
        var presenter = ActivatorUtilities.CreateInstance<GridUIBroker<DmoCustomer, CustomerId>>(_serviceProvider);
        return ValueTask.FromResult<IGridUIBroker<DmoCustomer>>(presenter);
    }
}
