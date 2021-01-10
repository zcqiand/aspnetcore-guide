using EventBus.MassTransit.Events;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace EventBus.MassTransit.Consumer.WebApi01
{
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
}
