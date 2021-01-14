# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* 什么是中介者模式

# 2 简介
.NET中的简单中介者模式实现，一种进程内消息传递机制（无其他外部依赖）。 支持以同步或异步的形式进行请求/响应，命令，查询，通知和事件的消息传递，并通过C#泛型支持消息的智能调度。

MediatR可以支持几种模式:请求/响应模式与发布模式。

# 3 请求/响应模式的使用
请求/响应模式，也可以叫做命令模式，是一对一的消息传递，一个消息对应一个消息处理。

**首先**，将MediatR,MediatR.Extensions.Microsoft.DependencyInjection的NuGet软件包安装到您的应用程序中。
```
MediatR
MediatR.Extensions.Microsoft.DependencyInjection
```

**然后**，我们通过Startup.ConfigureServices增加AddMediatR来注册组件。
```
services.AddMediatR(typeof(TenantRequestHandler).Assembly);
```

**接着**，定义消息类。
```
namespace MediatR.WebApi
{
    public class TenantRequest : IRequest<string>
    {
        public TenantRequest(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }
}
```

**接着**，定义消息处理类。
```
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.WebApi
{
    public class TenantRequestHandler : IRequestHandler<TenantRequest, string>
    {
        private readonly ILogger<TenantRequestHandler> logger;
        public TenantRequestHandler(ILogger<TenantRequestHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<string> Handle(TenantRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Handled: {request.Message}");
            return Task.FromResult(request.Message);
        }
    }
}
```

**最后**，创建HomeController，发布请求
```
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MediatR.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IMediator mediator;
        public HomeController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Route("Example01")]
        [HttpGet]
        public async Task<IActionResult> Example01()
        {
            await mediator.Send(new TenantRequest { Message= "通知房客信息" });
            return Ok();
        }
    }
}
```

项目中跟踪会输出：

```
Handled: 通知房客信息
```

# 4 发布模式的使用
发布模式，是一对多的消息传递，一个消息对应多个消息处理。

安装及注册组件后，**首先**，定义消息类。
```
namespace MediatR.WebApi
{
    public class LandlordNotification : INotification
    {
        public LandlordNotification(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }
}
```

**接着**，定义消息处理类。
```
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.WebApi
{
    public class LandlordNotificationHandler : INotificationHandler<LandlordNotification>
    {
        private readonly ILogger<LandlordNotificationHandler> logger;
        public LandlordNotificationHandler(ILogger<LandlordNotificationHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(LandlordNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Handled: {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
```

**最后**，在HomeController，增加发布请求
```
[Route("Example02")]
[HttpGet]
public async Task<IActionResult> Example02()
{
    await mediator.Publish(new LandlordNotification("通知房东信息"));
    return Ok();
}
```

项目中跟踪会输出：

```
Handled: 通知房东信息
```