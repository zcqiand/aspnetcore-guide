# 1 简介
RabbitMQ有成千上万的用户，是最受欢迎的开源消息代理之一。

## 1.1 AMQP是什么
AMQP（高级消息队列协议）是一个网络协议。它支持符合要求的客户端应用（application）和消息中间件代理（messaging middleware broker）之间进行通信。

## 1.2 消息队列是什么
MQ 全称为Message Queue, 消息队列。是一种应用程序对应用程序的通信方法。应用程序通过读写出入队列的消息（针对应用程序的数据）来通信，而无需专用连接来链接它们。

# 2 安装
通过docker进行安装

**首先**，进入RabbitMQ官网 http://www.rabbitmq.com/download.html
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210105190605.png)


**然后**，找到 Docker image 并进入
找到你需要安装的版本， -management 表示有管理界面的，可以浏览器访问。
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210105190722.png)

**接着**，接来下docker安装，我这里装的 3-management：
```
docker run -d --hostname my-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

**最后**，浏览器访问看下：http://localhost:15672/ ，用户名/密码： guest/guest
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210105190307.png)

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210105190446.png)


# 3 使用
## 3.1  “ Hello World！”
RabbitMQ是消息代理：它接受并转发消息。您可以将其视为邮局：将您要发布的邮件放在邮箱中时，可以确保邮递员先生或女士最终将邮件传递给收件人。
在下图中，“ P”是我们的生产者，“ C”是我们的消费者。中间的框是一个队列
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210106090209.png)

生产者代码：
```
using RabbitMQ.Client; //1. 使用名称空间
using System;
using System.Text;

namespace Example.RabbitMQ.HelloWorld.Producer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection()) //2. 创建到服务器的连接
            using (var channel = connection.CreateModel()) //3. 创建一个通道
            {
                channel.QueueDeclare(queue: "Example.RabbitMQ.HelloWorld", durable: false, exclusive: false, autoDelete: false, arguments: null); //4. 声明要发送到的队列

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: "Example.RabbitMQ.HelloWorld", basicProperties: null, body: body);//5. 将消息发布到队列

                Console.WriteLine(" 发送消息：{0}", message);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
```

消费者代码：使用命名空间，创建服务器连接，创建通道，声明队列都与生产者代码一致，增加了将队列中的消息传递给我们。由于它将异步地向我们发送消息，因此我们提供了回调。这就是EventingBasicConsumer.Received事件处理程序所做的。
```
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Example.RabbitMQ.HelloWorld.Consumer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Example.RabbitMQ.HelloWorld", durable: false, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine(" 等待消息。");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" 接收消息：{0}", message);
                };
                channel.BasicConsume(queue: "Example.RabbitMQ.HelloWorld", autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
```

让我们来看看输出结果:

发送端：
```
 发送消息：Hello World!
 Press [enter] to exit.
```

接收端：
```
 等待消息。
 Press [enter] to exit.
 接收消息：Hello World!
```

## 3.2  工作队列
工作队列（又称任务队列）的主要思想是避免立即执行资源密集型任务，然后必须等待其完成。相反，我们安排任务在以后完成。我们将任务封装为消息并将其发送到队列。工作进行在后台运行并不断的从队列中取出任务然后执行。当你运行了多个工作进程时，任务队列中的任务将会被工作进程共享执行。
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210106092335.png)

生产者代码：
```
using RabbitMQ.Client;
using System;
using System.Text;

namespace Example.RabbitMQ.WorkQueues.Producer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Example.RabbitMQ.WorkQueues", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "", routingKey: "Example.RabbitMQ.WorkQueues", basicProperties: properties, body: body);
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
```

消费者代码：
```
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Example.RabbitMQ.WorkQueues.Consumer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Example.RabbitMQ.WorkQueues", durable: true, exclusive: false, autoDelete: false, arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

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

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "Example.RabbitMQ.WorkQueues", autoAck: false, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
```

### 循环调度
使用任务队列的好处是能够很容易的并行工作。如果我们积压了很多工作，我们仅仅通过增加更多的工作者就可以解决问题，使系统的伸缩性更加容易。

让我们来看看输出结果:

发送端：
```
\bin\Debug\net5.0>Example.RabbitMQ.WorkQueues.Producer.ConsoleApp 消息1
 发送消息：消息1
 Press [enter] to exit.


\bin\Debug\net5.0>Example.RabbitMQ.WorkQueues.Producer.ConsoleApp 消息2
 发送消息：消息2
 Press [enter] to exit.


\bin\Debug\net5.0>Example.RabbitMQ.WorkQueues.Producer.ConsoleApp 消息3
 发送消息：消息3
 Press [enter] to exit.


\bin\Debug\net5.0>Example.RabbitMQ.WorkQueues.Producer.ConsoleApp 消息4
 发送消息：消息4
 Press [enter] to exit.
```

接收端1：
```
 等待消息。
 Press [enter] to exit.
 接收消息：消息1
 接收完成
 接收消息：消息3
 接收完成
```

接收端2：
```
 等待消息。
 Press [enter] to exit.
 接收消息：消息2
 接收完成
 接收消息：消息4
 接收完成
```

默认情况下，RabbitMQ将按顺序将每个消息发送给下一个使用者。平均而言，每个消费者都会收到相同数量的消息。这种分发消息的方式称为循环。与三个或更多的工人一起尝试。

### 消息确认
为了确保消息永不丢失，RabbitMQ支持消息确认。消费者发送回一个确认（acknowledgement），以告知RabbitMQ已经接收，处理了特定的消息，并且RabbitMQ可以自由删除它。
```
channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
```
使用此代码，我们可以确保，即使您在处理消息时使用CTRL + C杀死工作人员，也不会丢失任何信息。工人死亡后不久，所有未确认的消息将重新发送。

### 消息持久性
我们已经学会了如何确保即使消费者死亡，任务也不会丢失。但是，如果RabbitMQ服务器停止，我们的任务仍然会丢失。

当RabbitMQ退出或崩溃时，除非您告知不要这样做，否则它将忘记队列和消息。要确保消息不会丢失，需要做两件事：我们需要将队列和消息都标记为持久。

**首先**，我们需要确保该队列将在RabbitMQ节点重启后继续存在。为此，我们需要将其声明为持久的：
```
channel.QueueDeclare(queue: "Example.RabbitMQ.WorkQueues", durable: true, exclusive: false, autoDelete: false, arguments: null);
```

**最后**，我们需要将消息标记为持久性-通过将IBasicProperties.SetPersistent设置为true。
```
var properties = channel.CreateBasicProperties();
properties.Persistent = true;
```

### 公平派遣
我们可以将BasicQos方法与 prefetchCount = 1设置一起使用。这告诉RabbitMQ一次不要给工人一个以上的消息。换句话说，在处理并确认上一条消息之前，不要将新消息发送给工作人员。而是将其分派给不忙的下一个工作程序。
```
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
```

## 3.3  发布/订阅
在上一个教程中，我们创建了一个工作队列。工作队列背后的假设是，每个任务都恰好交付给一个工人。在这一部分中，我们将做一些完全不同的事情-我们将消息传达给多个消费者。这种模式称为“发布/订阅”。
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210106100434.png)


生产者代码：
```
using RabbitMQ.Client;
using System;
using System.Text;

namespace Example.RabbitMQ.PublishSubscribe.Producer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "Example.RabbitMQ.PublishSubscribe", type: ExchangeType.Fanout);

                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "Example.RabbitMQ.PublishSubscribe", routingKey: "", basicProperties: null, body: body);
                Console.WriteLine(" 发送消息：{0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "info: Hello World!";
        }
    }
}
```
生产者代码与上一教程看起来没有太大不同。最重要的变化是我们现在希望将消息发布到 Example.RabbitMQ.PublishSubscribe 交换器，而不是无名的消息交换器。交换类型有以下几种：direct，topic，headers 和fanout，在这里我们采用fanout交换类型。

消费者代码：
```
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Example.RabbitMQ.PublishSubscribe.Consumer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "Example.RabbitMQ.PublishSubscribe", type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "Example.RabbitMQ.PublishSubscribe", routingKey: "");

                Console.WriteLine(" 等待消息。");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" 接收消息：{0}", message);
                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
```
如果没有队列绑定到交换机，则消息将丢失，但这对我们来说是可以的。如果没有消费者在听，我们可以安全地丢弃该消息。

## 3.4  路由
在上一个教程中，我们创建了一个发布/订阅。我们能够向许多接收者广播消息。在本教程中，我们将向其中添加功能-将消息分类指定给具体的订阅者。
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210106102951.png)

生产者代码：
```
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace Example.RabbitMQ.Routing.Producer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "Example.RabbitMQ.Routing", type: "direct");

                var severity = (args.Length > 0) ? args[0] : "info";
                var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "Example.RabbitMQ.Routing", routingKey: severity, basicProperties: null, body: body);
                Console.WriteLine(" 发送消息：'{0}':'{1}'", severity, message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
```

消费者代码：
```
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Example.RabbitMQ.Routing.Consumer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "Example.RabbitMQ.Routing", type: "direct");
                var queueName = channel.QueueDeclare().QueueName;

                if (args.Length < 1)
                {
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }

                foreach (var severity in args)
                {
                    channel.QueueBind(queue: queueName, exchange: "Example.RabbitMQ.Routing", routingKey: severity);
                }

                Console.WriteLine(" 等待消息。");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" 接收消息：'{0}':'{1}'", routingKey, message);
                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
```

## 3.5  话题
在上一个教程中，我们改进了消息系统。代替使用仅能进行虚拟广播的扇出交换机，我们使用直接交换机，并有选择地接收消息的可能性。

尽管使用直接交换对我们的系统进行了改进，但它仍然存在局限性-它无法基于多个条件进行路由。

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210105/20210106103306.png)

*（星号）可以代替一个单词。
＃（哈希）可以替代零个或多个单词。

生产者代码：
```
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace Example.RabbitMQ.Topics.Producer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "Example.RabbitMQ.Topics", type: "topic");

                var routingKey = (args.Length > 0) ? args[0] : "info";
                var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "Example.RabbitMQ.Topics", routingKey: routingKey, basicProperties: null, body: body);
                Console.WriteLine(" 发送消息：'{0}':'{1}'", routingKey, message);
            }
        }
    }
}
```

消费者代码：
```
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Example.RabbitMQ.Topics.Consumer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "Example.RabbitMQ.Topics", type: "topic");
                var queueName = channel.QueueDeclare().QueueName;

                if (args.Length < 1)
                {
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }

                foreach (var bindingKey in args)
                {
                    channel.QueueBind(queue: queueName, exchange: "Example.RabbitMQ.Topics", routingKey: bindingKey);
                }

                Console.WriteLine(" 等待消息。 To exit press CTRL+C");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" 接收消息：'{0}':'{1}'", routingKey, message);
                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
```
