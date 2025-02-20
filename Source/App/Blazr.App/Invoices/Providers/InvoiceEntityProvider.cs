/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class InvoiceEntityProvider : IEntityProvider<DmoInvoice, InvoiceId>
{
    public Func<IMediator, InvoiceId, Task<Result<DmoInvoice>>> RecordRequest
        => (broker, id) => broker.Send(new InvoiceRequests.InvoiceRecordRequest(id));

    public Func<IMediator, DmoInvoice, CommandState, Task<Result<InvoiceId>>> RecordCommand
        => (broker, record, state) => Task.FromResult(Result<InvoiceId>.Fail(new Exception("You can't Update an Invoice this way")));

    public InvoiceId GetKey(object key)
    {
        return key switch
        {
            InvoiceId id => id,
            Guid guid => new InvoiceId(guid),
            _ => InvoiceId.Default
        };
    }

    public InvoiceId GetKey(DmoInvoice record)
    {
        return record.Id;
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
