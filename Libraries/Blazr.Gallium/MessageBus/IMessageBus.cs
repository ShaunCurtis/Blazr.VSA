/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Gallium;

public interface IMessageBus
{
    public void Publish<TMessage>(object? message);

    public void Subscribe<TMessage>(Action<object> callback);

    public void UnSubscribe<TMessage>(Action<object> callback);
}