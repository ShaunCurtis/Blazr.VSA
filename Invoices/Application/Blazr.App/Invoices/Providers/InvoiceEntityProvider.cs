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

public class InvoiceEntityProvider
   : EntityProvider<DmoInvoice>,
    IEntityProvider<DmoInvoice, InvoiceId>
{
    private readonly IMediatorBroker _mediator;
    private readonly IServiceProvider _serviceProvider;

    public InvoiceEntityProvider(IMediatorBroker mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public Task<Result<GridItemsProviderResult<DmoInvoice>>> GetItemsAsync(GridState<DmoInvoice> state)
        => InvoiceListRequest.Create(state)
            .DispatchAsync((request) => _mediator.DispatchAsync(request))
            .ExecuteFunctionAsync((itemsProvider) => GridItemsProviderResult
                .From<DmoInvoice>(itemsProvider.Items.ToList(), itemsProvider.TotalCount));

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

    public DmoInvoice NewRecord
        => new DmoInvoice { Id = InvoiceId.Default };

    public Func<InvoiceId, Task<Result<InvoiceEntity>>> EntityRequestAsync
        => (id) => id.IsDefault ? NewRecordRequestAsync(id) : ExistingRecordRequestAsync(id);

    public Func<StateRecord<InvoiceEntity>, Task<Result>> EntityCommandAsync
        => (record) => _mediator.DispatchAsync(new InvoiceCommandRequest(record));

    private Func<InvoiceId, Task<Result<InvoiceEntity>>> ExistingRecordRequestAsync
        => (id) => _mediator.DispatchAsync(new InvoiceRecordRequest(id));

    private Func<InvoiceId, Task<Result<InvoiceEntity>>> NewRecordRequestAsync
        => (id) => Task.FromResult(Result<InvoiceEntity>.Create(InvoiceEntity.Create()));
}
