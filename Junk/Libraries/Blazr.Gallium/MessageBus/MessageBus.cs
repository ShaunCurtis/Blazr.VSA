/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Threading.Tasks.Dataflow;

namespace Blazr.Gallium;

public class MessageBus : IMessageBus
{
    private readonly List<Subscription> _subscriptions = new();
    private readonly ActionBlock<Message> _queue;

    public MessageBus()
    {
        _queue = new(PublishMessage);
    }

    private readonly record struct Message(Subscription Subscription, object Value);

    private void PublishMessage(Message message)
    {
        message.Subscription.SubscriptionAction.Invoke(message.Value);
    }

    public void Subscribe<TMessage>(Action<object> callback)
    {
        var subscription = new Subscription(typeof(TMessage), callback);

        if (!_subscriptions.Contains(subscription))
            _subscriptions.Add(subscription);
    }

    public void Publish<TTarget>(object? message)
    {
        ArgumentNullException.ThrowIfNull(message);

        List<Subscription> handlers;

        lock (_subscriptions)
            handlers = _subscriptions.Where(item => item.SubscriptionType == typeof(TTarget)).ToList();

        foreach (var handler in handlers)
            _queue.Post(new(handler, message));
        //ThreadPool.QueueUserWorkItem((state) => handler.SubscriptionAction.Invoke(message));
    }

    public void UnSubscribe<TMessage>(Action<object> callback)
    {
        var subscription = new Subscription(typeof(TMessage), callback);

        if (_subscriptions.Contains(subscription))
            _subscriptions.Remove(subscription);
    }
}
