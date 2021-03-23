using EventBus.RabbitMQ.Events;
using RabbitMQ.Client;
using System;

namespace EventBus.RabbitMQ.Consumer.ConsoleApp01
{
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
            var eventBus = new RabbitMQEventBus(connectionFactory, "EventBus.RabbitMQ.Events.Exchange", queueName: "EventBus.RabbitMQ.Events.Queue");

            var customerASendedEventHandler = new CustomerASendedEventHandler();
            eventBus.Subscribe<SendedEvent>(customerASendedEventHandler);
            var customerBSendedEventHandler = new CustomerBSendedEventHandler();
            eventBus.Subscribe<SendedEvent>(customerBSendedEventHandler);

            Console.ReadKey();
        }
    }
}
