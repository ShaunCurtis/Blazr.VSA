/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.App.Presentation;

public interface IEditPresenter<TRecordEditContext, TKey>
        where TKey : notnull, IEntityId
{
    public IDataResult LastResult { get; }
    public TRecordEditContext EditMutator { get; }
    public EditContext EditContext { get; }
    public CommandState CommandState { get; }

    public ValueTask LoadAsync(TKey? id);
    public ValueTask ResetItemAsync();
    public ValueTask SaveItemAsync(bool refreshOnNew = true);
    public ValueTask DeleteItemAsync();
}
