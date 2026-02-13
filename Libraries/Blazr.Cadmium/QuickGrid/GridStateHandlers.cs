/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Cadmium.QuickGrid;

public readonly record struct UpdateGridRequest<TRecord>(int StartIndex, int PageSize, bool SortDescending, string? SortField)
    where TRecord : class
{
    public Result<UpdateGridRequest<TRecord>> ToReturnT() => ResultT.Read(this);

    public GridState<TRecord> ToGridState(Guid contextId) => new()
    {
        Key = contextId,
        StartIndex = this.StartIndex,
        PageSize = this.PageSize,
        SortDescending = this.SortDescending,
        SortField = this.SortField
    };
}

