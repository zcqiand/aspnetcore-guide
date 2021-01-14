# 1 Swagger是什么？
Swagger用于描述 REST API。 它允许计算机和人员了解服务的功能，而无需直接访问实现（源代码、网络访问、文档）。

# 2 安装
```
Swashbuckle.AspNetCore
```

# 添加Swagger生成器
将Swagger生成器添加到 Startup.ConfigureServices 方法中的服务集合中：

    services.AddSwaggerGen();

# 配置Swagger中间件
在 Startup.Configure 方法中，启用中间件为生成的 JSON 文档和 Swagger UI 提供服务：

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

# XML注释
- 在“解决方案资源管理器”中右键单击该项目，然后选择“编辑< project_name>.csproj” 。
- 手动将PropertyGroup添加：
```
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```


更改services.AddSwaggerGen();代码如下：

    services.AddSwaggerGen((c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    }));

# 效果
![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/200930/20200930133728.png)

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/200930/20200930133810.png)