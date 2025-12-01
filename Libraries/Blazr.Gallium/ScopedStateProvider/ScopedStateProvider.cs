/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Manganese;

namespace Blazr.Gallium;

public interface IScopedStateProvider<TKey, TData>
     where TData : class
{
    public Return Dispatch(TKey key, TData data);

    public Return ClearState(TKey key);

    public Return<T> GetState<T>(TKey key) where T : class;
}

public class ScopedStateProvider : IScopedStateProvider<Guid, object>
{
    private readonly record struct StateSubscription(object Data)
    {
        public readonly DateTime TimeStamp = DateTime.UtcNow;
    }

    private Dictionary<Guid, StateSubscription> _subscriptions = new();
    private TimeSpan StateTTL = TimeSpan.FromMinutes(60);

    public Return Dispatch(Guid key, object data)
    {
        if (_subscriptions.ContainsKey(key))
            _subscriptions[key] = new(data);
        else
            _subscriptions.Add(key, new(data));

        this.ClearExpiredStates();

        return Return.Success();
    }

    public Return<T> Dispatch<T>(T data) where T : class, IScopedState
        {
        if (_subscriptions.ContainsKey(data.Key))
            _subscriptions[data.Key] = new(data);
        else
            _subscriptions.Add(data.Key, new(data));

        this.ClearExpiredStates();

        return Return<T>.Success(data);
    }

    public Return ClearState(Guid key)
    {
        if (_subscriptions.ContainsKey(key))
            _subscriptions.Remove(key);

        return Return.Success();
    }

    public Return<T> GetState<T>(Guid key) where T : class
        => _subscriptions.ContainsKey(key)
            ? Return<T>.Read(_subscriptions[key].Data as T)
            : Return<T>.Failure($"No state found for key {key}");
        
    private void ClearExpiredStates()
    {
        var expiredStates = _subscriptions.Where(item => DateTime.UtcNow > item.Value.TimeStamp.AddTicks(StateTTL.Ticks)).Select(item => item.Key);
        foreach (var key in expiredStates)
            _subscriptions.Remove(key);
    }
}
