
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation;

namespace Blazr.App.Core;

public interface IEntityProvider<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public Func<TKey, Task<Result<TRecord>>> RecordRequest { get; }

    public Func<TRecord, CommandState, Task<Result<TKey>>> RecordCommand { get; }

    public Func<GridState<TRecord>, Task<Result<ListItemsProvider<TRecord>>>> ListRequest { get; }

    public TKey GetKey(object obj);

    public bool TryGetKey(object obj, out TKey key);

    public TRecord NewRecord { get; }
}
