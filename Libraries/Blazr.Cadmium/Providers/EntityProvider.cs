/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium;

public class EntityProvider<TRecord>
    where TRecord : class, new()
{
    public static Result<GridItemsProviderResult<TRecord>> FromListItemsProvider(ListItemsProvider<TRecord> itemsProvider)
        => Result<GridItemsProviderResult<TRecord>>
            .Create(GridItemsProviderResult
                .From<TRecord>(itemsProvider.Items.ToList(), itemsProvider.TotalCount));
}