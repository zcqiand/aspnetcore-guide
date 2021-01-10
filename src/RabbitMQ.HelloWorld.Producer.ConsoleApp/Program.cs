using RabbitMQ.Client; //1. 使用名称空间
using System;
using System.Text;

namespace RabbitMQ.HelloWorld.Producer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection()) //2. 创建到服务器的连接
            using (var channel = connection.CreateModel()) //3. 创建一个通道
            {
                channel.QueueDeclare(queue: "RabbitMQ.HelloWorld", durable: false, exclusive: false, autoDelete: false, arguments: null); //4. 声明要发送到的队列

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: "RabbitMQ.HelloWorld", basicProperties: null, body: body);//5. 将消息发布到队列

                Console.WriteLine(" 发送消息：{0}", message);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
