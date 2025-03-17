
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Presentation;
using static Blazr.App.Core.InvoiceRequests;

namespace Blazr.App.Core;

public class InvoiceEntityProvider : IEntityProvider<DmoInvoice, InvoiceId>
{
    private readonly IMediator _mediator;

    public Func<InvoiceId, Task<Result<DmoInvoice>>> RecordRequest
        => (id) => _mediator.Send(new InvoiceRequests.InvoiceRecordRequest(id));

    public Func<DmoInvoice, CommandState, Task<Result<InvoiceId>>> RecordCommand
        => (record, state) => Task.FromResult(Result<InvoiceId>.Fail(new Exception("You can't Update an Invoice this way")));

    public Func<GridState<DmoInvoice>, Task<Result<ListItemsProvider<DmoInvoice>>>> ListRequest
        => (state) => _mediator.Send(new InvoiceListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public InvoiceEntityProvider(IMediator mediator)
    {
        _mediator = mediator;
    }

    public InvoiceId GetKey(object obj)
    {
        return obj switch
        {
            InvoiceId id => id,
            DmoInvoice record => record.Id,
            Guid guid => new InvoiceId(guid),
            _ => InvoiceId.Default
        };
    }

    public bool TryGetKey(object obj, out InvoiceId key)
    {
        key = GetKey(obj);
        return key != InvoiceId.Default;
    }

    public DmoInvoice NewRecord
        => DefaultRecord;

    public static DmoInvoice DefaultRecord
        => new DmoInvoice
        {
            Id = InvoiceId.Default,
            Date = DateOnly.FromDateTime(DateTime.Now)
        };
}
