# 需求
“待办事项”的功能清单：
- 获取所有待办事项
- 按 ID 获取项
- 添加新项
- 更新现有项
- 删除项

# 创建 Web 项目
- 从“文件”菜单中选择“新建”>“项目” 。
- 选择“ASP.NET Core Web 应用程序”模板，再单击“下一步” 。
- 将项目命名为 App001，然后单击“创建”。
- 在“创建新的 ASP.NET Core Web 应用程序”对话框中，确认选择“.NET Core”和“ASP.NET Core 3.1” 。 选择“API”模板，然后单击“创建” 。

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/200929/20200929172030.png)

# 测试 API
按 Ctrl+F5 运行应用。 Visual Studio 启动浏览器并导航到 https://localhost:<port>/WeatherForecast，其中 <port> 是随机选择的端口号。

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/201005/20201005215735.png)

到目前为止，小明已经成功创建并运行了一个WebApi项目。

# 项目结构
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/201005/20201005222440.png)

从这个图中可以看出WebApi项目主要由Program.cs，Startup.cs，appsettings.json，WeatherForecastController.cs文件组成，那么现在我们一个一个介绍一下这几个文件主要由什么作用。

## Program类
它是所有.net core程序的入口，定义了2个方法：Main() 和CreateHostBuilder()；
```
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```
代码不复杂，创建泛型主机并运行。

## Startup类
主要包括 ConfigureServices 方法以配置应用的服务和Configure 方法以创建应用的请求处理管道。

```
// 运行时将调用此方法。 使用此方法将服务添加到容器。
public void ConfigureServices(IServiceCollection services)
{
}

// 运行时将调用此方法。 使用此方法来配置HTTP请求管道。
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
}
```

## appsettings.json
appsettings.json是在core中的配置文件，类似与以前asp.net中的web.config

## WeatherForecastController.cs
就是一个控制器，就是处理 Web API 请求，派生自 ControllerBase 的控制器类。

```
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
```

# 发布到IIS
## 在 Windows Server 上安装.NET Core Hosting Bundle。
https://dotnet.microsoft.com/download/dotnet-core/3.1

## 创建 IIS 站点
1. 在 IIS 服务器上，创建一个文件夹以包含应用已发布的文件夹和文件。 在接下来的步骤中，文件夹路径作为应用程序的物理路径提供给 IIS。
2. 在 IIS 管理器中，打开“连接”面板中的服务器节点。 右键单击“站点”文件夹。 选择上下文菜单中的“添加网站”。
3. 提供网站名称，并将“物理路径”设置为所创建应用的部署文件夹 。 提供“绑定”配置，并通过选择“确定”创建网站 。

## 发布和部署应用
1. 将应用发布到一个文件夹。
2. 文件夹的内容将移动到 IIS 站点的文件夹（IIS 管理器中站点的物理路径）。