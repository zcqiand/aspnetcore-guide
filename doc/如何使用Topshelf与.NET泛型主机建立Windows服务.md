# 1 前置阅读
在阅读本文章之前，你可以先阅读：
* Topshelf一个用于使用.NET构建Windows服务框架

# 2 使用
## 2.1 创建应用程序
**首先**，创建一个新的控制台应用程序并从nuget获取Topshelf和Microsoft.Extensions.Hosting软件包
```
Topshelf
Microsoft.Extensions.Hosting
```

当然我们也需要安装Serilog相关的日志框架。
```
Serilog.Extensions.Hosting
Serilog.Settings.Configuration
Serilog.Sinks.Console
Serilog.Sinks.File
Topshelf.Serilog
```

## 2.2 创建.NET泛型主机
**然后**，我们先建立CreateHostBuilder()方法，并加载了Serilog日志并依赖注入MyService和AppSettings，MyService类做为Topshelf所使用的主要逻辑程序，它会提供Start()和Stop()做为Topshelf执行或停止主要逻辑程序的动作。
```
class Program
{
    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<AppSettings>(hostContext.Configuration);
                services.AddTransient<MyService>();
            });
}
```

## 2.3 在Topshelf中注册服务
**接着**，在Topshelf中注册我们的服务类。跳转到Program.cs并添加：
```
class Program
{
    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        RunWindowsServiceWithHost(host);
    }

    private static void RunWindowsServiceWithHost(IHost host)
    {
        var rc = HostFactory.Run(x =>
        {
            x.UseSerilog();
            x.SetDisplayName("我的服务");
            x.SetDescription("我的服务详细描述");
            x.SetServiceName("MyService");

            var myService = host.Services.GetRequiredService<MyService>();
            x.Service<MyService>(s =>
            {
                s.ConstructUsing(() => myService);
                s.WhenStarted(tc => tc.Start());
                s.WhenStopped(tc => tc.Stop());
            });
            x.RunAsLocalSystem();
            x.StartAutomatically();
        });

        var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
        Environment.ExitCode = exitCode;
    }
}
```

## 2.4 MyService类
**接着**，我们看看MyService类，主要演示了注入ILogger和AppSettings。
```
public class MyService
{
    private readonly ILogger logger;
    private readonly AppSettings settings;

    public MyService(IOptions<AppSettings> settings, ILogger<MyService> logger)
    {
        this.settings = settings.Value;
        this.logger = logger;
    }
    public void Start()
    {
        logger.LogInformation($"Starting {this.settings.ServiceName}...");
    }

    public void Stop()
    {
        logger.LogInformation($"Stopping {this.settings.ServiceName}...");
    }
}
```

## 2.5 运行应用程序
**最后**，F5执行应用程序，如果一切顺利，你应该会看到类似以下内容的信息：
![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210402103636.png)