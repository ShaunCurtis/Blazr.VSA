/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public record ListQueryRequest<TRecord>
{
    public int StartIndex { get; init; }
    public int PageSize { get; init; }
    public Expression<Func<TRecord, bool>>? FilterExpression { get; init; }
    public Expression<Func<TRecord, object>>? SortExpression { get; init; }
    public bool SortDescending { get; init; } = true;
    public CancellationToken Cancellation { get; init; }

    public ListQueryRequest()
    {
        StartIndex = 0;
        PageSize = 1000;
        FilterExpression = null;
        SortExpression = null;
        Cancellation = new();
    }
    public static ListQueryRequest<TRecord> Create(CancellationToken? cancellationToken = null)
        => new ListQueryRequest<TRecord>() { Cancellation = cancellationToken ?? new() };
}
