/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.Cadmium.QuickGrid;

public class SortedPropertyColumn<TGridItem> : TemplateColumn<TGridItem>
{
    [Parameter] public string? SortField { get; set; }
}