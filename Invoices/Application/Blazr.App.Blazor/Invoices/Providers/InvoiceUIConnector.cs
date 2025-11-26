/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;
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

    public Func<InvoiceId, Task<Bool<DmoInvoice>>> RecordRequestAsync
        => throw new NotImplementedException();

    public Func<StateRecord<DmoInvoice>, Task<Bool<InvoiceId>>> RecordCommandAsync
        => throw new NotImplementedException();

    public Func<GridState<DmoInvoice>, Task<Bool<ListItemsProvider<DmoInvoice>>>> GridItemsRequestAsync
        => (state) => _mediator.DispatchAsync(new InvoiceListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public Task<Bool<GridItemsProviderResult<DmoInvoice>>> GetItemsAsync(GridState<DmoInvoice> state)
        => InvoiceListRequest.Create(state)
            .BindAsync((request) => _mediator.DispatchAsync(request))
            .MapAsync((itemsProvider) => GridItemsProviderResult
                    .From<DmoInvoice>(itemsProvider.Items
                    .ToList(), 
                itemsProvider.TotalCount));

    public Func<InvoiceListRequest, Task<Bool<ListItemsProvider<DmoInvoice>>>> ListItemsRequestAsync
        => (request) => _mediator.DispatchAsync(request);

    public Bool<InvoiceId> GetKey(object? obj)
        => obj switch
        {
            InvoiceId id => BoolT.Read(id),
            DmoInvoice record => BoolT.Read(record.Id),
            Guid guid => BoolT.Read(new InvoiceId(guid)),
            _ => Bool<InvoiceId>.Failure($"Could not convert the provided key - {obj?.ToString()}")
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
