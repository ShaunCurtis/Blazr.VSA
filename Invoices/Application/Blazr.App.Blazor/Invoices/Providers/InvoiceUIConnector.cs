/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using Blazr.Manganese;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public class InvoiceUIConnector
   : UIConnector<DmoInvoice>,
    IUIConnector<DmoInvoice, InvoiceId>
{
    private readonly IMediatorBroker _mediator;
    private readonly IServiceProvider _serviceProvider;

    public string SingleDisplayName { get; } = "Invoice";
    public string PluralDisplayName { get; } = "Invoices";
    public Type? EditForm { get; } = null;
    public Type? ViewForm { get; } = null;
    public string Url { get; } = "/Invoice";

    public InvoiceUIConnector(IMediatorBroker mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public Func<InvoiceId, Task<Result<DmoInvoice>>> RecordRequestAsync
        => throw new NotImplementedException();

    public Func<StateRecord<DmoInvoice>, Task<Result<InvoiceId>>> RecordCommandAsync
        => throw new NotImplementedException();

    public Func<GridState<DmoInvoice>, Task<Result<ListItemsProvider<DmoInvoice>>>> GridItemsRequestAsync
        => (state) => _mediator.DispatchAsync(new InvoiceListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public Task<Result<GridItemsProviderResult<DmoInvoice>>> GetItemsAsync(GridState<DmoInvoice> state)
        => InvoiceListRequest.Create(state)
            .ExecuteTransformAsync((request) => _mediator.DispatchAsync(request))
            .MapAsync((itemsProvider) => GridItemsProviderResult
                .From<DmoInvoice>(itemsProvider.Items.ToList(), itemsProvider.TotalCount));

    public Func<InvoiceListRequest, Task<Result<ListItemsProvider<DmoInvoice>>>> ListItemsRequestAsync
        => (request) => _mediator.DispatchAsync(request);

    public Result<InvoiceId> GetKey(object? obj)
        => obj switch
        {
            InvoiceId id => Result<InvoiceId>.Create(id),
            DmoInvoice record => Result<InvoiceId>.Create(record.Id),
            Guid guid => Result<InvoiceId>.Create(new(guid)),
            _ => Result<InvoiceId>.Failure($"Could not convert the provided key - {obj?.ToString()}")
        };

    public IRecordMutor<DmoInvoice> GetRecordMutor(DmoInvoice record)
    {
        throw new NotImplementedException();
    }

    public IRecordMutor<DmoInvoice> GetNewRecordMutor()
    {
        throw new NotImplementedException();
    }

    public DmoInvoice NewRecord
        => new DmoInvoice { Id = InvoiceId.Default };
}
