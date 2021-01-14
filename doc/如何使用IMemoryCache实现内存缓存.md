# 1 缓存基础知识
缓存是实际工作中非常常用的一种提高性能的方法。

缓存可以减少生成内容所需的工作，从而显著提高应用程序的性能和可伸缩性。 缓存最适用于不经常更改的数据。 通过缓存，可以比从原始数据源返回的数据的副本速度快得多。 

# 2 使用内存缓存（IMemoryCache）
**首先**，我们简单的创建一个控制器，实现一个简单方法，返回当前时间。我们可以看到每次访问这个接口，都可以看到当前时间。

```
[Route("api/[controller]")]
[ApiController]
public class CacheController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return DateTime.Now.ToString();
    }
}
```

**然后**，将Microsoft.Extensions.Caching.Memory的NuGet软件包安装到您的应用程序中。
```
Microsoft.Extensions.Caching.Memory
```

**接着**，使用依赖关系注入从应用中引用的服务，在Startup类的ConfigureServices()方法中配置：

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMemoryCache();
}
```

**接着**，在构造函数中请求IMemoryCache实例
```
private IMemoryCache cache;
public CacheController(IMemoryCache cache)
{
    this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
}
```

**最后**，在Get方法中使用缓存
```
[HttpGet]
public string Get()
{
    //读取缓存
    var now = cache.Get<string>("cacheNow");
    if (now == null) //如果没有该缓存
    {
        now = DateTime.Now.ToString();
        cache.Set("cacheNow", now);
        return now;
    }
    else
    {
        return now;
    }
}
```

经过测试可以看到，缓存后，我们取到日期就从内存中获得，而不需要每次都去计算，说明缓存起作用了。