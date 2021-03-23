using EventBus;
using EventBus.RabbitMQ;
using RabbitMQ.Client;
using System;

namespace EventBus.RabbitMQ.ConsoleApp01
{
    public class SendedEvent : IEvent
    {
        public string Name { get; private set; }
        public SendedEvent(string name)
        {
            Name = name;
        }
    }

    public class CustomerASendedEventHandler : IEventHandler<SendedEvent>
    {
        public void Handle(SendedEvent @event)
        {
            Console.WriteLine($"顾客A收到{@event.Name}通知！");
        }
    }

    public class CustomerBSendedEventHandler : IEventHandler<SendedEvent>
    {
        public void Handle(SendedEvent @event)
        {
            Console.WriteLine($"顾客B收到{@event.Name}通知！");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory { HostName = "localhost" };
            var eventBus = new RabbitMQEventBus(connectionFactory, "EventBus.RabbitMQ.ConsoleApp01.Exchange", queueName: "EventBus.RabbitMQ.ConsoleApp01.Queue");

            var sendedEvent = new SendedEvent("优惠");

            var customerASendedEventHandler = new CustomerASendedEventHandler();
            eventBus.Subscribe<SendedEvent>(customerASendedEventHandler);
            var customerBSendedEventHandler = new CustomerBSendedEventHandler();
            eventBus.Subscribe<SendedEvent>(customerBSendedEventHandler);
            Console.WriteLine($"商店发了{sendedEvent.Name}通知！");
            eventBus.Publish<SendedEvent>(sendedEvent);

            Console.ReadKey();
        }
    }
}
