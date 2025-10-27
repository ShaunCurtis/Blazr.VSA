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
    public Type? EditForm { get; } = null;
    public Type? ViewForm { get; } = null;
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

    public ValueTask<IGridUIBroker<DmoCustomer>> GetGridUIBrokerAsync()
    {
        var presenter = ActivatorUtilities.CreateInstance<GridUIBroker<DmoCustomer, CustomerId>>(_serviceProvider);
        return ValueTask.FromResult<IGridUIBroker<DmoCustomer>>(presenter);
    }

    public async ValueTask<IEditUIBroker<TEditContext, CustomerId>> GetEditUIBrokerAsync<TEditContext>(CustomerId id)
        where TEditContext : IRecordEditContext<DmoCustomer>, new()
    {
        var presenter = ActivatorUtilities.CreateInstance<EditUIBroker<DmoCustomer, TEditContext, CustomerId>>(_serviceProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }

    public ValueTask<IGridUIBroker<DmoCustomer>> GetGridUIBrokerAsync(Guid contextId)
        => throw new NotImplementedException();

    public ValueTask<IGridUIBroker<DmoCustomer>> GetGridUIBrokerAsync(Guid contextId, UpdateGridRequest<DmoCustomer> resetGridRequest)
        => throw new NotImplementedException();
}
