/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public interface IDataResult
{
    public bool Successful { get; }
    public string? Message { get; }
}
