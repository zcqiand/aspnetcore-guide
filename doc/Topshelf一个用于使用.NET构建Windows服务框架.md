# 1 Topshelf是什么？
Topshelf是用于托管使用.NET框架编写的Windows服务的框架。服务的创建得到简化，从而使开发人员可以创建一个简单的控制台应用程序，可以使用Topshelf将其作为服务安装。原因很简单：调试控制台应用程序比服务容易得多。一旦对应用程序进行了测试并准备投入生产，Topshelf便可以轻松地将应用程序即服务安装。

# 2 使用
## 2.1 创建应用程序
**首先**，创建一个新的控制台应用程序并从nuget获取Topshelf软件包
```
Topshelf
```

当您使用Topshelf时，我还建议装一下日志库，我们可以选择日志框架Serilog。
```
Topshelf.Serilog
Serilog.Sinks.Console
```

## 2.2 创建服务类
**然后**，创建服务类。我将其命名为MyService。在这里放置将在Windows服务的特定生命周期事件中调用的方法。至少，添加一些用于启动和停止服务的方法。
```
public class MyService
{
    readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
    public void Start()
    {
        log.Information("Starting MyService...");
    }

    public void Stop()
    {
        log.Information("Stopping MyService...");
    }
}
```

## 2.3 在Topshelf中注册服务
**接着**，在Topshelf中注册我们的服务类。跳转到Program.cs并添加：
```
class Program
{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();

        var rc = HostFactory.Run(x =>
        {
            x.UseSerilog(); // HostLogger改为使用Serilog。
            x.SetDisplayName("我的服务");  // 我们设置要在Windows服务监视器中使用的winservice的显示名称。
            x.SetDescription("我的服务详细描述"); // 我们设置了在Windows服务监视器中使用的winservice的描述。
            x.SetServiceName("MyService"); // 我们设置要在Windows服务监视器中使用的winservice的服务名称。
            x.Service<MyService>(s =>
            {
                s.ConstructUsing(name => new MyService()); // 构建服务实例。
                s.WhenStarted(tc => tc.Start()); // 启动服务
                s.WhenStopped(tc => tc.Stop()); // 停止服务
            });
            x.RunAsLocalSystem(); // 设置“登录为”并选择了“本地系统”。
            x.StartAutomatically(); // 设置“启动类型”并选择了“自动”。
        });

        var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode()); // 我们转换并返回服务退出代码。
        Environment.ExitCode = exitCode;
    }
}
```

## 2.4 运行应用程序
**接着**，F5执行应用程序，如果一切顺利，你应该会看到类似以下内容的信息：
![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210402103636.png)


## 2.5 安装Windows服务
**最后**，安装Windows服务，以管理员身份打开命令行，浏览到exe目录并使用install参数调用它：
```
S045.Topshelf.ConsoleApp.exe install
```
现在，您可以在“服务”窗口中签出新注册的Windows服务。
![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210322/20210402104009.png)