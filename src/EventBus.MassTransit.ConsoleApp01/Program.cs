using MassTransit;
using System;
using System.Threading.Tasks;

namespace EventBus.MassTransit.ConsoleApp01
{
    /// <summary>
    /// 具体事件源类
    /// </summary>
    public class SendedEvent
    {
        public string Name { get; private set; }
        public SendedEvent(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 具体事件处理类顾客A
    /// </summary>
    public class CustomerASendedEventHandler : IConsumer<SendedEvent>
    {
        public Task Consume(ConsumeContext<SendedEvent> context)
        {
            Console.WriteLine($"顾客A收到{context.Message.Name}通知！");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 具体事件处理类顾客B
    /// </summary>
    public class CustomerBSendedEventHandler : IConsumer<SendedEvent>
    {
        public Task Consume(ConsumeContext<SendedEvent> context)
        {
            Console.WriteLine($"顾客B收到{context.Message.Name}通知！");
            return Task.CompletedTask;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host("rabbitmq://localhost");

                sbc.ReceiveEndpoint("event-listener", ep =>
                {
                    ep.Consumer<CustomerASendedEventHandler>();
                    ep.Consumer<CustomerBSendedEventHandler>();
                });
            });

            bus.Start();

            var sendedEvent = new SendedEvent("优惠");
            Console.WriteLine($"商店发了{sendedEvent.Name}通知！");
            bus.Publish(sendedEvent);
            Console.ReadKey();
            bus.Stop();
        }
    }
}
