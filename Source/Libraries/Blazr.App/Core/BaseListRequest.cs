/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record BaseListRequest
{
    public int StartIndex { get; init; } = 0;
    public int PageSize { get; init; } = 1000;
    public string? SortColumn { get; init; } = null;
    public bool SortDescending { get; init; } = false;
}
