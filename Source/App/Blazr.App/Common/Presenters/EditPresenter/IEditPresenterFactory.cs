/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IEditPresenterFactory<TRecordEditContext, TKey>
        where TKey : notnull, IEntityId
{
    public ValueTask<IEditPresenter<TRecordEditContext, TKey>> GetPresenterAsync(TKey id);
}

public interface IEditPresenterFactory
{
    public ValueTask<IEditPresenter<TRecordEditContext, TKey>> GetPresenterAsync<TRecordEditContext, TKey>(TKey id)
                where TKey : notnull, IEntityId;
}
