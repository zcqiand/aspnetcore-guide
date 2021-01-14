# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* 什么是依赖注入

# 2 简介
Microsoft.Extensions.DependencyInjection是.NET Core内置依赖注入模块。

# 3 使用
**首先**，在Startup.ConfigureServices方法中，将Knife，Actor注册到服务容器中。
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<Actor>();
    services.AddTransient<Knife>();
    
    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Example.DependencyInjection.WebApi", Version = "v1" });
    });
}
```

**然后**，增加HomeController,执行actor.Kill。
```
using Microsoft.AspNetCore.Mvc;
using System;

namespace Autofac.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {

        private readonly Actor actor;
        public HomeController(Actor actor)
        {
            this.actor = actor ?? throw new ArgumentNullException(nameof(actor));
        }

        [HttpGet]
        public string Get()
        {
            return actor.Kill();
        }
    }
}
```

启动调试，让我们来看看输出结果:
```
小明用刀杀怪
```

# 4 生命周期
* 单例 Singleton：依赖项注入容器对服务实现的每个后续请求都使用相同的实例。
* 作用域 Scoped：对于Web应用程序，作用域范围内的生存期表示每个客户端请求（连接）都会创建一次服务。
* 瞬时（暂时）Transient：每次从服务容器中请求时，都会创建瞬态生存期服务。

**首先**，分别建三个代表生命周期的类，MySingletonService，MyScopedService，MyTransientService。
```
namespace Example.DependencyInjection.WebApi
{
    public class MySingletonService
    {
    }
    public class MyScopedService
    {
    }
    public class MyTransientService
    {
    }
}
```

**然后**，在Startup.ConfigureServices方法中，将MySingletonService，MyScopedService，MyTransientService注册到服务容器中。
```
services.AddSingleton<MySingletonService, MySingletonService>();
services.AddScoped<MyScopedService, MyScopedService>();
services.AddTransient<MyTransientService, MyTransientService>();
```

**最后**，HomeController增加GetServiceLifetime方法。
```
[Route("ServiceLifetime")]
[HttpGet]
public List<string> GetServiceLifetime([FromServices] MySingletonService singleton1,
    [FromServices] MySingletonService singleton2,
    [FromServices] MyScopedService scoped1,
    [FromServices] MyScopedService scoped2,
    [FromServices] MyTransientService transient1,
    [FromServices] MyTransientService transient2)
{
    var s = new List<string>();
    s.Add($"singleton1:{singleton1.GetHashCode()}");
    s.Add($"singleton2:{singleton2.GetHashCode()}");
    s.Add($"scoped1:{scoped1.GetHashCode()}");
    s.Add($"scoped2:{scoped2.GetHashCode()}");
    s.Add($"transient1:{transient1.GetHashCode()}");
    s.Add($"transient2:{transient2.GetHashCode()}");
    return s;
}
```

启动调试，执行两遍，让我们来看看输出结果:
第一遍：
```
[
  "singleton1:65122748",
  "singleton2:65122748",
  "scoped1:52786977",
  "scoped2:52786977",
  "transient1:16782441",
  "transient2:16991442"
]
```

第二遍：
```
[
  "singleton1:65122748",
  "singleton2:65122748",
  "scoped1:56140151",
  "scoped2:56140151",
  "transient1:1997173",
  "transient2:54718731"
]
```

从结果我们发现：
* 单例 Singleton：两次的 HashCode 没有变化
* 作用域 Scoped：每个请求内 HashCode 是相同的，不同的请求的 HashCode 是不同的
* 瞬时（暂时）Transient：每次的 HashCode 都不同

注意例子中，我们使用通过 [FromServices] 注入的，另外我们也可以选择通过 controller 构造函数注入。