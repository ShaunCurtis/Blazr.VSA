/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium.Core;

public interface IEntityProvider<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public Task<Result<GridItemsProviderResult<TRecord>>> GetItemsAsync(GridState<TRecord> state);

    public Func<TKey, Task<Result<TRecord>>> RecordRequestAsync { get; }

    public Func<StateRecord<TRecord>, Task<Result<TKey>>> RecordCommandAsync { get; }

    public Func<GridState<TRecord>, Task<Result<ListItemsProvider<TRecord>>>> GridItemsRequestAsync { get; }

    public Result<TKey> GetKey(object? obj);

    public TRecord NewRecord { get; }
}
