/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony;

public record ListQueryRequest<TRecord>
{
    public int StartIndex { get; init; }
    public int PageSize { get; init; }
    public CancellationToken Cancellation { get; init; }
    public Expression<Func<TRecord, bool>>? FilterExpression { get; init; }
    public Expression<Func<TRecord, object>>? SortExpression { get; init; }
    public bool SortDescending { get; init; } = true;

    public ListQueryRequest()
    {
        StartIndex = 0;
        PageSize = 1000;
        Cancellation = new();
        FilterExpression = null;
        SortExpression = null;
    }
}