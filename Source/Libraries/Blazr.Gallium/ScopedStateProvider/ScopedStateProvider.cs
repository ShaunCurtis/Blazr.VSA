/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Gallium;

public interface IScopedStateProvider<TKey, TData>
     where TData : class
{
    public void Dispatch(TKey key, TData data);

    public void ClearState(TKey key);

    public TData? GetState(TKey key);

    public bool TryGetState(TKey key, out TData? value);

    public T? GetState<T>(TKey key) where T : class;

    public bool TryGetState<T>(TKey key, out T? value) where T : class;
}

public class ScopedStateProvider : IScopedStateProvider<Guid, object>
{
    private readonly record struct StateSubscription(object Data)
    {
        public readonly DateTime TimeStamp = DateTime.UtcNow;
    }

    private Dictionary<Guid, StateSubscription> _subscriptions = new();
    private TimeSpan StateTTL = TimeSpan.FromMinutes(60);

    public void Dispatch(Guid key, object data)
    {
        if (_subscriptions.ContainsKey(key))
            _subscriptions[key] = new(data);
        else
            _subscriptions.Add(key, new(data));

        this.ClearExpiredStates();
    }

    public void ClearState(Guid key)
    {
        if (_subscriptions.ContainsKey(key))
            _subscriptions.Remove(key);
    }

    public T? GetState<T>(Guid key) where T : class
    {
        if (_subscriptions.ContainsKey(key))
            return _subscriptions[key].Data as T;

        return default;
    }

    public bool TryGetState<T>(Guid key, [NotNullWhen(true)] out T? value) where T : class
    {
        value = null;

        if (_subscriptions.ContainsKey(key))
            value = _subscriptions[key].Data as T;

        return value is not null;
    }

    private void ClearExpiredStates()
    {
        var expiredStates = _subscriptions.Where(item => DateTime.UtcNow > item.Value.TimeStamp.AddTicks(StateTTL.Ticks)).Select(item => item.Key);
        foreach (var key in expiredStates)
            _subscriptions.Remove(key);
    }

    public object? GetState(Guid key)
    {
        if (_subscriptions.ContainsKey(key))
            return _subscriptions[key].Data;

        return default;
    }

    public bool TryGetState(Guid key, [NotNullWhen(true)] out object? data)
    {
        data = _subscriptions.ContainsKey(key)
            ? _subscriptions[key].Data
            : default;

        return data is not null;
    }
}
