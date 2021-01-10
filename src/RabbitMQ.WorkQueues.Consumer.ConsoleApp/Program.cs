using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.WorkQueues.Consumer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "RabbitMQ.WorkQueues", durable: true, exclusive: false, autoDelete: false, arguments: null); // 消息持久性

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);// 公平派遣

                Console.WriteLine(" 等待消息。");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" 接收消息：{0}", message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" 接收完成");
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);// 消息确认
                };
                channel.BasicConsume(queue: "RabbitMQ.WorkQueues", autoAck: false, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
