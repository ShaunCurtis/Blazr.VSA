/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Gallium;

namespace Blazr.App.Presentation;

public class ReadUIBroker<TRecord, TKey> : IReadUIBroker<TRecord, TKey>, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;
    private readonly IMessageBus _messageBus;
    private TKey _key = default!;

    public TRecord Item { get; protected set; } = new TRecord();
    public event EventHandler? RecordChanged;
    public IDataResult LastResult { get; protected set; } = DataResult.Success();

    public ReadUIBroker(IEntityProvider<TRecord, TKey> entityProvider, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _entityProvider = entityProvider;

        _messageBus.Subscribe<TKey>(OnRecordChanged);
    }

    public async ValueTask LoadAsync(TKey id)
        => await GetRecordItemAsync(id);

    private async ValueTask GetRecordItemAsync(TKey id)
    {
        _key = id;

        // Call the RecordRequest on the record specific EntityProvider to get the record
        var result = await _entityProvider.RecordRequest.Invoke(id);

        LastResult = result.ToDataResult;

        if (result.HasSucceeded(out TRecord? record))
            this.Item = record ?? _entityProvider.NewRecord;
    }

    private async void OnRecordChanged(object? obj)
    {
        // test to see if we have a key of the same type
        // if so and it doesn't match the current key, we dont need to do anything
        if ( _entityProvider.TryGetKey(obj ?? new(), out TKey key) && !key.Equals(_key))
                return;

        // We either have a matching  key or don't know so load the record just in case
        await this.GetRecordItemAsync(_key);

        this.RecordChanged?.Invoke(this, EventArgs.Empty);

        return;
    }

    public void Dispose()
    {
        _messageBus.UnSubscribe<TKey>(OnRecordChanged);
    }
}