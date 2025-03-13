/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IReadPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public TRecord Item { get; }
    
    public IDataResult LastResult { get;}

    public ValueTask LoadAsync(TKey id);
}