/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IGridState<TRecord>
    where TRecord : class
{
    public int PageSize { get; }
    public int StartIndex { get;}
    public bool SortDescending { get; }
    public string? SortField { get; }
}
