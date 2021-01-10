# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* RabbitMQ入门
* 什么是观察者模式
* 什么是事件总线

# 2 实现
**首先**，事件源与事件处理的映射字典。
```
private static Dictionary<string, List<object>> eventHandlers = new Dictionary<string, List<object>>();
```

**然后**，初始化RabbitMQ,创建到服务器的连接,创建一个通道等
```
public RabbitMQEventBus(IConnectionFactory connectionFactory,
    string exchangeName,
    string exchangeType = ExchangeType.Fanout,
    string queueName = null,
    bool autoAck = false)
{
    this.connectionFactory = connectionFactory;
    this.connection = this.connectionFactory.CreateConnection();
    this.channel = this.connection.CreateModel();
    this.exchangeType = exchangeType;
    this.exchangeName = exchangeName;
    this.autoAck = autoAck;

    this.channel.ExchangeDeclare(this.exchangeName, this.exchangeType);

    this.queueName = this.InitializeEventConsumer(queueName);
}
```

**接着**，实现订阅，往字典表中添加事件处理实例，并绑定队列
```
public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
{
    var eventTypeName = typeof(TEvent).FullName;
    if (eventHandlers.ContainsKey(eventTypeName))
    {
        var handlers = eventHandlers[eventTypeName];
        handlers.Add(eventHandler);
    }
    else
    {
        eventHandlers.Add(eventTypeName, new List<object> { eventHandler });
    }
    this.channel.QueueBind(this.queueName, this.exchangeName, typeof(TEvent).FullName);
}
```

**接着**，实现取消订阅，从字典表中删除事件处理实例，并取消绑定队列
```
public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
{

    var eventType = typeof(TEvent).FullName;
    if (eventHandlers.ContainsKey(eventType))
    {
        var handlers = eventHandlers[eventType];
        if (handlers != null && handlers.Exists(s => s.GetType() == eventHandler.GetType()))
        {
            var handlerToRemove = handlers.First(s => s.GetType() == eventHandler.GetType());
            handlers.Remove(handlerToRemove);

            this.channel.QueueUnbind(this.queueName, this.exchangeName, typeof(TEvent).FullName);
        }
    }
}
```

**接着**，实现发布，往队列发布事件
```
public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
{
    var json = JsonConvert.SerializeObject(@event, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
    var eventBody = Encoding.UTF8.GetBytes(json);
    channel.BasicPublish(this.exchangeName,
        @event.GetType().FullName,
        null,
        eventBody);
}
```

**接着**，在EventingBasicConsumer.Received事件处理中，通过事件源找到对应的事件处理类，并执行它
```
private string InitializeEventConsumer(string queue)
{
    var localQueueName = queue;
    if (string.IsNullOrEmpty(localQueueName))
    {
        localQueueName = this.channel.QueueDeclare().QueueName;
    }
    else
    {
        this.channel.QueueDeclare(localQueueName, true, false, false, null);
    }

    var consumer = new EventingBasicConsumer(this.channel);
    consumer.Received += (model, eventArgument) =>
    {
        var eventBody = eventArgument.Body.ToArray();
        var json = Encoding.UTF8.GetString(eventBody);
        var @event = (IEvent)JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        var eventTypeName = eventArgument.RoutingKey;

        if (eventHandlers.ContainsKey(eventTypeName))
        {
            var handlers = eventHandlers[eventTypeName];
            try
            {
                foreach (var handler in handlers)
                {
                    MethodInfo meth = handler.GetType().GetMethod("Handle");
                    meth.Invoke(handler, new Object[] { @event });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        if (!autoAck)
        {
            channel.BasicAck(eventArgument.DeliveryTag, false);
        }
    };

    this.channel.BasicConsume(localQueueName, autoAck: this.autoAck, consumer: consumer);

    return localQueueName;
}
```

**最后**，创建客户端类，具体事件源类，具体事件处理类。
```
using Example.EventBus;
using RabbitMQ.Client;
using System;

namespace Eaxmple.EventBus.RabbitMQ.ConsoleApp01
{
    public class SendedEvent : IEvent
    {
        public string Name { get; private set; }
        public SendedEvent(string name)
        {
            Name = name;
        }
    }

    public class CustomerASendedEventHandler : IEventHandler<SendedEvent>
    {
        public void Handle(SendedEvent @event)
        {
            Console.WriteLine($"顾客A收到{@event.Name}通知！");
        }
    }

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
            var connectionFactory = new ConnectionFactory { HostName = "localhost" };
            var eventBus = new RabbitMQEventBus(connectionFactory, "Eaxmple.EventBus.RabbitMQ.ConsoleApp01.Exchange", queueName: "Eaxmple.EventBus.RabbitMQ.ConsoleApp01.Queue");

            var sendedEvent = new SendedEvent("优惠");

            var customerASendedEventHandler = new CustomerASendedEventHandler();
            eventBus.Subscribe<SendedEvent>(customerASendedEventHandler);
            var customerBSendedEventHandler = new CustomerBSendedEventHandler();
            eventBus.Subscribe<SendedEvent>(customerBSendedEventHandler);
            Console.WriteLine($"商店发了{sendedEvent.Name}通知！");
            eventBus.Publish<SendedEvent>(sendedEvent);

            Console.ReadKey();
        }
    }
}
```

让我们来看看输出结果:
```
商店发布优惠通知！
顾客A收到优惠通知。
顾客B收到优惠通知。
```