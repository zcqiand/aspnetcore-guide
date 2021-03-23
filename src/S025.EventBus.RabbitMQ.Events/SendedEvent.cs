namespace EventBus.RabbitMQ.Events
{
    public class SendedEvent : IEvent
    {
        public string Name { get; private set; }
        public SendedEvent(string name)
        {
            Name = name;
        }
    }
}
