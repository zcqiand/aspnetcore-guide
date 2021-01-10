# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* 什么是依赖注入

# 2 简介
Autofac与C#语言的结合非常紧密，并学习它非常的简单，也是.NET领域最为流行的IoC框架之一。

# 3 使用
**首先**，将Autofac的NuGet软件包安装到您的应用程序中。
```
Autofac
```

**然后**，我们通过创建ContainerBuilder来注册组件。
```
var builder = new ContainerBuilder();
builder.RegisterType<Knife>();
builder.RegisterType<Actor>();
```

**接着**，可以通过在一个已存在的生命周期上调用 BeginLifetimeScope() 方法来创建另一个生命周期作用域, 以根容器作为起始。生命周期作用域是可释放的并且追踪组件的释放, 因此确保你总是调用了 "Dispose()"" 或者把它们包裹在 "using" 语句内。
```
using (var scope = container.BeginLifetimeScope())
{
}
```

**最后**，在注册完组件并暴露相应的服务后, 你可以从创建的容器或其子生命周期中解析服务. 让我们使用 Resolve() 方法来实现:
```
using (var scope = container.BeginLifetimeScope())
{
    var actor = scope.Resolve<Actor>();
    actor.Kill();
}
```

让我们来看看完整代码:
```
using System;

namespace Autofac.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Knife>();
            builder.RegisterType<Actor>();

            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var actor = scope.Resolve<Actor>();
                actor.Kill();
            }

            Console.ReadKey();
        }
    }
}
```

让我们来看看输出结果:
```
小明用刀杀怪
```

# 4 在 Asp.Net Core 中使用
**首先**，将Autofac,Autofac.Extensions.DependencyInjection的NuGet软件包安装到您的应用程序中。
```
dotnet add package Autofac
dotnet add package Autofac.Extensions.DependencyInjection
```

**然后**，在Program.Main中增加.UseServiceProviderFactory(new AutofacServiceProviderFactory())
```
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());
```

**接着**，在Startup.ConfigureServices中增加services.AddControllersWithViews();
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Autofac.WebApi", Version = "v1" });
    });

    services.AddControllersWithViews();
}
```

**接着**，在Startup.ConfigureContainer方法中，将Knife，Actor注册到Autofac中ContainerBuilder。
```
public void ConfigureContainer(ContainerBuilder builder)
{
    builder.RegisterType<Knife>();
    builder.RegisterType<Actor>();
}
```

**最后**，增加HomeController,执行actor.Kill。
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