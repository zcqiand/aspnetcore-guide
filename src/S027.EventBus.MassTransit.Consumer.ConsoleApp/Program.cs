using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.MassTransit.Consumer.ConsoleApp01
{
    class Program
    {
        static void Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ReceiveEndpoint("event-listener", e =>
                {
                    e.Consumer<CustomerASendedEventHandler>();
                    e.Consumer<CustomerBSendedEventHandler>();
                });
            });
            busControl.Start();
            try
            {
                Console.ReadKey();
            }
            finally
            {
                busControl.Stop();
            }
        }
    }
}
