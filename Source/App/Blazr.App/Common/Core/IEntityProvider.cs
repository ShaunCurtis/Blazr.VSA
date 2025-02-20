/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;
public interface IEntityProvider<TRecord, TKey>
{
    public Func<IMediator, TKey, Task<Result<TRecord>>> RecordRequest { get; }

    public Func<IMediator, TRecord, CommandState, Task<Result<TKey>>> RecordCommand { get; }

    public TKey GetKey(object key);

    public TKey GetKey(TRecord record);

    public TRecord NewRecord { get; }
}
