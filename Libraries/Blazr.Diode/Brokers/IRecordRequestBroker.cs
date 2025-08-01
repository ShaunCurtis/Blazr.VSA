/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public interface IRecordRequestBroker
{
    public ValueTask<Result<TRecord>> ExecuteAsync<TRecord>(RecordQueryRequest<TRecord> request)
        where TRecord : class;
}
