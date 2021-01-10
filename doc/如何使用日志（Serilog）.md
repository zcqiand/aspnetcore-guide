# 1 Serilog是什么？
在.NET使用日志框架第一时间会想到NLog或是Log4Net，Serilog 是这几年快速崛起的Log框架之一，Serilog是以Structured logging 为基础进行设计，透过logging API 可以轻松的记录应用程式中对象属性，方便快速进行logging 内容进行查询与分析，并将其纪录内容透过json (可指定) 的方式输出。

# 2 使用
**首先**，将Serilog.AspNetCore NuGet软件包安装到您的应用程序中。
```
Serilog.AspNetCore
```

**然后**，在应用程序的Program.cs文件中，首先配置Serilog。
```
public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

public static void Main(string[] args)
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

    try
    {
        CreateHostBuilder(args).Build().Run();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "主机意外终止");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog() // <-添加此行
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

**接着**，更改appsettings.json配置：删除"Logging"节点，增加"Serilog"节点
```
"Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      { "Name": "Console" }
    ]
  },
```


**接着**，控制器的构造函数中请求logger实例
```
private readonly ILogger<LogController> logger; // <-添加此行
public LogController(ILogger<LogController> logger)
{
    this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**最后**，在Get方法中使用Log
```
[HttpGet]
public void Get()
{
    logger.LogInformation("测试1"); // <-添加此行
}
```