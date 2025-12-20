/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public readonly record struct UpdateGridRequest<TRecord>(int StartIndex, int PageSize, bool SortDescending, string? SortField)
{
    public static UpdateGridRequest<TRecord> Create(GridItemsProviderRequest<TRecord> request)
    {
        var column = request.SortByColumn as SortedPropertyColumn<TRecord>;

        if (column is not null)
            return new()
            {
                StartIndex = request.StartIndex,
                PageSize = request.Count ?? 0,
                SortDescending = !request.SortByAscending,
                SortField = column?.SortField ?? null,
            };

        var definedSorters = request.GetSortByProperties();

        return new()
        {
            StartIndex = request.StartIndex,
            PageSize = request.Count ?? 0,
            SortDescending = definedSorters?.FirstOrDefault().Direction == Microsoft.AspNetCore.Components.QuickGrid.SortDirection.Descending,
            SortField = definedSorters?.FirstOrDefault().PropertyName ?? null,
        };
    }
}

