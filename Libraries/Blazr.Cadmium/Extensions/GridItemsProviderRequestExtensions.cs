/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.QuickGrid;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium
{
    public static class GridItemsProviderRequestExtensions
    {
        extension<TRecord>(GridItemsProviderRequest<TRecord> @this) where TRecord : class
        {
            public UpdateGridRequest<TRecord> ConvertToUpdateGridRequest(bool defaultSortDescending = false, string? defaultSortField = null)
            {
                var column = @this.SortByColumn as SortedPropertyColumn<TRecord>;

                if (column is not null)
                    return new()
                    {
                        StartIndex = @this.StartIndex,
                        PageSize = @this.Count ?? 10,
                        SortDescending = !@this.SortByAscending,
                        SortField = column?.SortField ?? null,
                    };

                var definedSorters = @this.GetSortByProperties();

                if (definedSorters != null && definedSorters.Any())
                {
                    return new()
                    {
                        StartIndex = @this.StartIndex,
                        PageSize = @this.Count ?? 10,
                        SortDescending = definedSorters?.FirstOrDefault().Direction == Microsoft.AspNetCore.Components.QuickGrid.SortDirection.Descending,
                        SortField = definedSorters?.FirstOrDefault().PropertyName ?? null,
                    };
                }

                return new()
                {
                    StartIndex = @this.StartIndex,
                    PageSize = @this.Count ?? 10,
                    SortDescending = defaultSortDescending,
                    SortField = defaultSortField,
                };
            }
        }
    }
}
