# 1 什么是ELK？
ELK，是Elastaicsearch、Logstash和Kibana三款软件的简称。Elastaicsearch是一个开源的全文搜索引擎。Logstash则是一个开源的数据收集引擎，具有实时的管道，它可以动态地将不同的数据源的数据统一起来。Kibana是一个日志可视化分析的平台，它提供了一系列日志分析的Web接口，可以使用它对日志进行高效地搜索、分析和可视化操作。我们可以定义ELK是一个集日志收集、搜索、日志聚合和日志分析于一身的完整解决方案。

# 3 如何使用ELK？
**首先**，安装ELK，以Docker方式安装。
```
docker pull sebp/elk
docker run -p 5601:5601 -p 9200:9200 -p 5044:5044 --name elk sebp/elk
```

**然后**，我们可以在浏览器中输入地址：http//localhost:9200，这是Elasticsearch的默认端口。我们可以获取关于Elasticseach的信息：
```
{
  "name" : "6a2c8682fba8",
  "cluster_name" : "docker-cluster",
  "cluster_uuid" : "dAGvy0BoTju-23eOlQWmGw",
  "version" : {
    "number" : "7.9.2",
    "build_flavor" : "default",
    "build_type" : "docker",
    "build_hash" : "d34da0ea4a966c4e49417f2da2f244e3e97b4e6e",
    "build_date" : "2020-09-23T00:45:33.626720Z",
    "build_snapshot" : false,
    "lucene_version" : "8.6.2",
    "minimum_wire_compatibility_version" : "6.8.0",
    "minimum_index_compatibility_version" : "6.0.0-beta1"
  },
  "tagline" : "You Know, for Search"
}
```

**接着**，我们继续在浏览器中输入地址：http://localhost:5601/app/kibana。我们可以看到Kibana的界面：

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210115/uOQSCUxfWYManK6.png)


**接着**，我们通过 Serilog 来收集日志信息，创建 ELK.WebApi01 项目，并在项目中引入三个依赖项：Serilog.AspNetCore和Serilog.Sinks.ElasticSearch。
```
Serilog.AspNetCore
Serilog.Sinks.ElasticSearch
```

**接着**，Program文件中增加：
```
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;

namespace ELK.WebApi01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .MinimumLevel.Debug()
              .WriteTo.Elasticsearch(
              new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
              {
                  MinimumLogEventLevel = LogEventLevel.Verbose,
                  AutoRegisterTemplate = true
              })
              .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
    }
}
```

**接着**，创建LogController，增加日志测试接口并执行它:
```
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ELK.WebApi01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogController : Controller
    {
        private readonly ILogger<LogController> logger; // <-添加此行
        public LogController(ILogger<LogController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public void Get()
        {
            logger.LogInformation("测试1"); // <-添加此行
        }
    }
}
```

**最后**，我们要到那里去找这些日志信息呢？我们在Kibana中点击左侧导航栏最底下的设置按钮，然后再点击右侧的Create index pattern按钮创建一个索引。什么叫做索引呢？在Elasticsearch中索引相当于一张”表”，如图：

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210115/20210121140656.png)

创建索引的时候，会发现列表中列出了目前Elasticsearch中可用的数据。这里的logstash-2020.02.15就是本文中的ASP.NET Core应用产生的日志信息。

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210115/20210121140808.png)

创建完索引，就可以看到目前收集的日志信息了，在此基础上，我们可以做进一步的检索、过滤，来生成各种各样的“查询”。而每一个“查询”实际上就是一个数据源。

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210115/20210121140552.png)