namespace EventBus
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;

        void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent;

        void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent;
    }
}
