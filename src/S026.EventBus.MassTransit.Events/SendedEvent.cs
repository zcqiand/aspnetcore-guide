namespace EventBus.MassTransit.Events
{
    public class SendedEvent
    {
        public string Name { get; private set; }
        public SendedEvent(string name)
        {
            Name = name;
        }
    }
}
