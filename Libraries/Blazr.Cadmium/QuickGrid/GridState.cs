/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;
using Blazr.Gallium;

namespace Blazr.Cadmium.QuickGrid;

public record GridState<TRecord> : IGridState<TRecord>, IScopedState
    where TRecord : class
{
    public Guid Key { get; init; }
    public int PageSize { get; init; }
    public int StartIndex { get; init; }
    public bool SortDescending { get; init; }
    public string? SortField { get; init; }
    public int Page
        => this.StartIndex / this.PageSize;

    public GridState()
    {
        this.PageSize = 1000;
        this.StartIndex = 0;
        this.SortDescending = false;
        this.SortField = null;
        this.Key = Guid.NewGuid();
    }

    public static GridState<TRecord> Create(Guid contextUid, int pageSize = 1000, int startIndex = 0, bool sortDescending = false, string? sortField = null)
    {
        return new GridState<TRecord>
        {
            Key = contextUid,
            PageSize = pageSize,
            StartIndex = startIndex,
            SortDescending = sortDescending,
            SortField = sortField
        };
    }
}

public static class GridStateExtensions
{
    public static Return<GridState<TRecord>> ToBoolT<TRecord>(this GridState<TRecord> state)
    where TRecord : class
        => Return<GridState<TRecord>>.Read(state);

    public static async Task<Return<ListItemsProvider<TRecord>>> ExecuteFunctionOnException<TRecord>(this GridState<TRecord> state, Func<GridState<TRecord>, Task<Return<ListItemsProvider<TRecord>>>> mapper)
        where TRecord : class
        => await mapper(state);

    public static Return<ListItemsProvider<TRecord>> ExecuteFunction<TRecord>(this GridState<TRecord> state, Func<GridState<TRecord>, Return<ListItemsProvider<TRecord>>> mapper)
    where TRecord : class
        => mapper(state);
}

