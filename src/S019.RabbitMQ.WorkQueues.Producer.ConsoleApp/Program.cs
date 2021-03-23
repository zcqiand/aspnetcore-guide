using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.WorkQueues.Producer.ConsoleApp
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

                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true; // 消息持久性

                channel.BasicPublish(exchange: "", routingKey: "RabbitMQ.WorkQueues", basicProperties: properties, body: body);
                Console.WriteLine(" 发送消息：{0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "Hello World!";
        }
    }
}
