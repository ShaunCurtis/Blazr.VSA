/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Blazr.Gallium;
using System.ComponentModel.DataAnnotations;

namespace Blazr.Cadmium.Presentation;

public partial class ReadUIBroker<TRecord, TKey> : IReadUIBroker<TRecord, TKey>, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public TRecord Item { get; protected set; } = new TRecord();
    public event EventHandler? RecordChanged;
    public Result LastResult { get; protected set; } = Result.Success();

    public ReadUIBroker(IEntityProvider<TRecord, TKey> entityProvider, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _entityProvider = entityProvider;

        _messageBus.Subscribe<TKey>(OnRecordChanged);
    }

    public async ValueTask LoadAsync(TKey id)
        => LastResult = await GetRecordItemAsync(id);

    public void Dispose()
    {
        _messageBus.UnSubscribe<TKey>(OnRecordChanged);
    }
}

public partial class ReadUIBroker<TRecord, TKey> : IReadUIBroker<TRecord, TKey>, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    private const string _sameRecordMessage = "The record requested is the same as already loaded";
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;
    private readonly IMessageBus _messageBus;
    private TKey _key = default!;

    private async Task<Result> GetRecordItemAsync(TKey id)
        => await Result<TKey>.Create(id)
            .SwitchToExceptionOnTrue(id.IsDefault, "The record Id is default.  Mo record retrieved.")
            .ExecuteAction((recordId) => _key = recordId)
            .ExecuteTransformAsync(_entityProvider.RecordRequestAsync)
            .ExecuteActionAsync(hasValue: (record) => this.Item = record)
            .ToResultAsync();

    private async void OnRecordChanged(object? obj)
    {
        // Get the key and update the record and trigger a RecordChanged event which will update the UI
        var result = await _entityProvider.GetKey(obj)
            .ExecuteTransformAsync(this.GetRecordItemAsync)
            .ExecuteActionAsync(hasValue: () => this.RecordChanged?.Invoke(this, EventArgs.Empty));
    }
}