/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public record RecordQueryRequest<TRecord>(
    Expression<Func<TRecord, bool>> FindExpression,
    CancellationToken? Cancellation = null
);
