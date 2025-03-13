/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class ReadPresenter<TRecord, TKey> : IReadPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    protected readonly IMediator _dataBroker;
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;

    public TRecord Item { get; protected set; } = new TRecord();

    public IDataResult LastResult { get; protected set; } = DataResult.Success();

    public ReadPresenter(IMediator dataBroker, IEntityProvider<TRecord, TKey> entityProvider)
    {
        _dataBroker = dataBroker;
        _entityProvider = entityProvider;
    }

    public async ValueTask LoadAsync(TKey id)
        => await GetRecordItemAsync(id);

    private async ValueTask GetRecordItemAsync(TKey id)
    {
        var result = await _entityProvider.RecordRequest.Invoke(_dataBroker, id);
        
        LastResult = result.ToDataResult;

        if (result.HasSucceeded(out TRecord? record))
            this.Item = record ?? _entityProvider.NewRecord; 
    }
}