﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public record RecordQueryRequest<TRecord>(
    Expression<Func<TRecord, bool>> FindExpression,
    CancellationToken? Cancellation = null
);
