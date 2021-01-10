# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* RabbitMQ入门
* 什么是观察者模式
* 什么是事件总线
* 如何使用RabbitMQ实现事件总线

# 2 简介
MassTransit 是一个自由、开源、轻量级的消息总线, 用于使用. NET 框架创建分布式应用程序。MassTransit 在现有消息传输上提供了一组广泛的功能, 从而使开发人员能够友好地使用基于消息的会话模式异步连接服务。基于消息的通信是实现面向服务的体系结构的可靠和可扩展的方式。

# 3 使用
**首先**，将MassTransit的NuGet软件包安装到您的应用程序中。
```
MassTransit
MassTransit.RabbitMQ
```

**然后**，创建具体事件源类。
```
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
```

**接着**，创建具体事件处理类，默认继承IConsumer<>接口。
```
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
```

**最后**，客户端调用。
```
static void Main(string[] args)
{
    var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
    {
        sbc.Host("rabbitmq://localhost");

        sbc.ReceiveEndpoint("test_queue", ep =>
        {
            ep.Consumer<CustomerASendedEventHandler>();// 订阅
            ep.Consumer<CustomerBSendedEventHandler>();
        });
    });

    bus.Start();

    var sendedEvent = new SendedEvent("优惠");
    Console.WriteLine($"商店发了{sendedEvent.Name}通知！");
    bus.Publish(sendedEvent);// 发布

    Console.ReadKey(); // press Enter to Stop
    bus.Stop();
}
```

让我们来看看输出结果:
```
商店发布优惠通知！
顾客A收到优惠通知。
顾客B收到优惠通知。
```

# 4 ASP.NET Core 使用
**首先**，将MassTransit的NuGet软件包安装到您的应用程序中。
```
MassTransit
MassTransit.RabbitMQ
MassTransit.AspNetCore
```

**然后**，创建具体事件源类
```
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
```

**接着**，创建发布者代码:

1. 往Startup.ConfigureServices中增加以下代码
```
services.AddMassTransit(x =>
{
    x.UsingRabbitMq();
});
services.AddMassTransitHostedService();
```

2.创建HomeController，发布事件
```
using EventBus.MassTransit.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventBus.MassTransit.Producer.WebApi01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;
        public HomeController(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            await publishEndpoint.Publish<SendedEvent>(new SendedEvent("优惠"));
            return Ok();
        }
    }
}
```

**接着**，创建订阅者代码:

1. 创建具体事件处理类
```
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
```

2. 往Startup.ConfigureServices中增加以下代码
```
services.AddMassTransit(x =>
{
    x.AddConsumer<CustomerASendedEventHandler>();
    x.AddConsumer<CustomerBSendedEventHandler>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("event-listener", e =>
        {
            e.ConfigureConsumer<CustomerASendedEventHandler>(context);
            e.ConfigureConsumer<CustomerBSendedEventHandler>(context);
        });
    });
});

services.AddMassTransitHostedService();
```

**最后**，执行Post()方法，在订阅者项目中跟踪会输出：

```
顾客A收到优惠通知。
顾客B收到优惠通知。
```
