/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.Cadmium.Presentation;

public interface IEditUIBroker<TRecord, TRecordMutor, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
    where TRecordMutor : IRecordMutor<TRecord>
{
    public Result LastResult { get; }
    public TRecordMutor EditMutator { get; }
    public EditContext EditContext { get; }
    public EditState State { get; }

    public ValueTask<Result> LoadAsync(TKey id);
    public ValueTask<Result> ResetAsync();
    public ValueTask<Result> SaveAsync(bool refreshOnNew = true);
    public ValueTask<Result> DeleteAsync();
}
