using Blazr.Diode;
using Microsoft.AspNetCore.Components.QuickGrid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazr.Cadmium.QuickGrid;

public static class QuickGridExtensions
{
    extension<TRecord>(ListItemsProvider<TRecord> @this)
    {
        public GridItemsProviderResult<TRecord> ToGridItemsProviderResult()
            => GridItemsProviderResult
                .From<TRecord>(@this.Items
                    .ToList(),
                    @this.TotalCount);
    }
}
