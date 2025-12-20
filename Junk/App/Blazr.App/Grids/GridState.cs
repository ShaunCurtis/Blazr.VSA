/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public record GridState<TRecord> : IGridState<TRecord>
    where TRecord : class
{
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
    }
}

