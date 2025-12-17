/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium.Presentation;

public interface IUIConnector<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public string SingleDisplayName { get; }
    public string PluralDisplayName { get; }
    public Type? EditForm { get; }
    public Type? ViewForm { get; }
    public string Url { get; }

    public Task<Return<GridItemsProviderResult<TRecord>>> GetItemsAsync(GridState<TRecord> state);

    public Func<TKey, Task<Return<TRecord>>> RecordRequestAsync { get; }

    public Func<TRecord, EditState, Task<Return<TKey>>> RecordCommandAsync { get; }

    public IRecordMutor<TRecord> GetRecordMutor(TRecord record);
}
