using System;

namespace EventBus.Sample.ConsoleApp01
{
    /// <summary>
    /// 具体事件源类
    /// </summary>
    public class SendedEvent : IEvent
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
    public class CustomerASendedEventHandler : IEventHandler<SendedEvent>
    {
        public void Handle(SendedEvent @event)
        {
            Console.WriteLine($"顾客A收到{@event.Name}通知！");
        }
    }

    /// <summary>
    /// 具体事件处理类顾客B
    /// </summary>
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
            var eventBus = new SampleEventBus();

            var sendedEvent = new SendedEvent("优惠");

            var customerASendedEventHandler = new CustomerASendedEventHandler();
            eventBus.Subscribe<SendedEvent>(customerASendedEventHandler);
            var customerBSendedEventHandler = new CustomerBSendedEventHandler();
            eventBus.Subscribe<SendedEvent>(customerBSendedEventHandler);
            Console.WriteLine($"商店发了{sendedEvent.Name}通知！");
            eventBus.Publish<SendedEvent>(sendedEvent);

            Console.WriteLine();

            eventBus.Unsubscribe<SendedEvent>(customerBSendedEventHandler);
            Console.WriteLine($"商店发了{sendedEvent.Name}通知！");
            eventBus.Publish<SendedEvent>(sendedEvent);

            Console.ReadKey();
        }
    }
}
