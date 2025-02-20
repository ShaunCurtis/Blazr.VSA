/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

/// <summary>
/// Result object used to return a list request up the data pipeline
/// While it's principly used in data paging,
/// all requests should be constrained 
/// </summary>
/// <typeparam name="TRecord"></typeparam>
/// <param name="Items"></param>
/// <param name="TotalCount"></param>
public readonly record struct  ListResult<TRecord>(IEnumerable<TRecord> Items, int TotalCount);
