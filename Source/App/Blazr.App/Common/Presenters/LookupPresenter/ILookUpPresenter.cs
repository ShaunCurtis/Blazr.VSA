/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface ILookUpPresenter<TItem>
    where TItem : class, ILookupItem, new()
{
    public IEnumerable<TItem> Items { get; }

    public ValueTask<Result> LoadAsync();
}
