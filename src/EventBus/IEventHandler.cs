namespace EventBus
{
    /// <summary>
    /// 事件处理
    /// </summary>
    /// <typeparam name="TEvent">事件源</typeparam>
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}
