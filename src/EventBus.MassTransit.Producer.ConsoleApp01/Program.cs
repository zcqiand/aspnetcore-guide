using EventBus.MassTransit.Events;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.MassTransit.Producer.ConsoleApp01
{
    class Program
    {
        static void Main(string[] args)
        {
            var sendedEvent = new SendedEvent("优惠");
            var busControl = Bus.Factory.CreateUsingRabbitMq();
            busControl.Start();
            try
            {
                Console.WriteLine($"商店发了{sendedEvent.Name}通知！");
                busControl.Publish(sendedEvent);
                Console.ReadKey();
            }
            finally
            {
                busControl.Stop();
            }
        }
    }
}
