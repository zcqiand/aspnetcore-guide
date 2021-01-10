using EventBus.RabbitMQ.Events;
using RabbitMQ.Client;
using System;

namespace EventBus.RabbitMQ.Producer.ConsoleApp01
{   

    class Program
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory { HostName = "localhost" };
            var eventBus = new RabbitMQEventBus(connectionFactory, "EventBus.RabbitMQ.Events.Exchange", queueName: "EventBus.RabbitMQ.Events.Queue");

            var sendedEvent = new SendedEvent("优惠");

            Console.WriteLine($"商店发了{sendedEvent.Name}通知！");
            eventBus.Publish<SendedEvent>(sendedEvent);

            Console.ReadKey();
        }
    }
}
