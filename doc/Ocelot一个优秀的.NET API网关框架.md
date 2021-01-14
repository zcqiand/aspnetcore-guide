# 1 什么是Ocelot？
Ocelot是一个用.NET Core实现并且开源的API网关，它功能强大，包括了：路由、请求聚合、服务发现、认证、鉴权、限流熔断、并内置了负载均衡器与Service Fabric、Butterfly Tracing集成。

# 2 如何使用Ocelot？
**首先**，创建2个WebApi项目，WebApi01和WebApi02，地址分别https://localhost:44313和https://localhost:44390，其中WebApi01当作网关，WebApi02当作具体的微服务Api。

**然后**，将Ocelot的NuGet软件包安装到WebApi01项目中。
```
Ocelot
```
注意我这里安装的是17.0.0版本，配置方面会有点不一样。

**接着**，在Startup.ConfigureServices中增加services.AddOcelot;
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Autofac.WebApi", Version = "v1" });
    });

    services.AddOcelot();
}
```

**接着**，在Startup.Configure中增加app.UseOcelot().Wait();
```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ocelot.WebApi01 v1"));
    }

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    app.UseOcelot().Wait();
}
```

**接着**，创建ocelot.json文件
```
{
  "Routes": [ //路由配置（注16.1版本将ReRoutes换成Routes）
    {
      "DownstreamPathTemplate": "/{url}", // 下游（服务提供方）服务路由模板
      "DownstreamScheme": "https", // 下游Uri方案，http、https
      "DownstreamHostAndPorts": [ // 服务地址和端口，如果是集群就设置多个
        {
          "Host": "localhost",
          "Port": 44390
        }
      ],
      "UpstreamPathTemplate": "/api/{url}", // 上游（客户端，服务消费方）请求路由模板
      "UpstreamHttpMethod": [ "GET" ] // 允许的上游HTTP请求方法，可以写多个
    }
  ],
  "GlobalConfiguration": { //全局配置
    "BaseUrl": "https://localhost:44313" //网关对外地址
  }
}
```

**最后**，在Program.CreateHostBuilder中增加AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
```
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, builder) => {
            builder.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

Ok,让我们来测试看看，https://localhost:44313/api/WeatherForecast会不会跳转https://localhost:44390/WeatherForecast。