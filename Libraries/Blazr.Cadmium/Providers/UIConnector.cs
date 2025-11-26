/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium;

public class UIConnector<TRecord>
    where TRecord : class, new()
{
    public static Bool<GridItemsProviderResult<TRecord>> FromListItemsProvider(ListItemsProvider<TRecord> itemsProvider)
        => Bool<GridItemsProviderResult<TRecord>>
            .Read(GridItemsProviderResult
                .From<TRecord>(itemsProvider.Items.ToList(), itemsProvider.TotalCount));
}