﻿/// ============================================================
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
    public static Result<GridState<TRecord>> ToResult<TRecord>(this GridState<TRecord> state)
    where TRecord : class
        => Result<GridState<TRecord>>.Create(state);

    public static async Task<Result<ListItemsProvider<TRecord>>> ApplyTransformOnException<TRecord>(this GridState<TRecord> state, Func<GridState<TRecord>, Task<Result<ListItemsProvider<TRecord>>>> mapper)
        where TRecord : class
        => await mapper(state);

    public static Result<ListItemsProvider<TRecord>> ApplyTransform<TRecord>(this GridState<TRecord> state, Func<GridState<TRecord>, Result<ListItemsProvider<TRecord>>> mapper)
    where TRecord : class
        => mapper(state);
}

