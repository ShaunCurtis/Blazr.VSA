/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Antimony;

public interface IListRequestBroker
{
    public ValueTask<Result<ListItemsProvider<TRecord>>> ExecuteAsync<TRecord>(ListQueryRequest<TRecord> request)
        where TRecord : class;
}

public interface IListRequestBroker<TRecord>
    where TRecord : class
{
    public ValueTask<Result<ListItemsProvider<TRecord>>> ExecuteAsync(ListQueryRequest<TRecord> request);
}
