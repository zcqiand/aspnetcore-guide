# 1 分布式缓存是什么
分布式缓存是由多个应用服务器共享的缓存，通常作为外部服务在访问它的应用服务器上维护。 分布式缓存可以提高 ASP.NET Core 应用程序的性能和可伸缩性，尤其是在应用程序由云服务或服务器场托管时。

# 2 Redis是什么？
Redis是一个高性能的 key-value 数据库。Redis性能极高，能读的速度是110000次/s,写的速度是81000次/s。

# 3 Redis 安装
这里我们不具体展开，你可以参考 https://www.runoob.com/redis/redis-install.html 按步骤进行安装。

# 4 使用 Redis 分布式缓存
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

**然后**，将Microsoft.Extensions.Caching.Redis的NuGet软件包安装到您的应用程序中。
```
Microsoft.Extensions.Caching.Redis
```

**接着**，使用依赖关系注入从应用中引用的服务，在Startup类的ConfigureServices()方法中配置：
```
public void ConfigureServices(IServiceCollection services)
{
    // install-package Microsoft.Extensions.Caching.Redis
    services.AddDistributedRedisCache(options =>
    {
        options.InstanceName = "";
        options.Configuration = "127.0.0.1:6379";
    });
}
```

**接着**，控制器的构造函数中请求IDistributedCache实例
```
private IDistributedCache cache;
public RedisCacheController(IDistributedCache cache)
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
    var now = cache.Get("cacheNow");
    if (now == null) //如果没有该缓存
    {
        cache.Set("cacheNow", Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
        now = cache.Get("cacheNow");
        return Encoding.UTF8.GetString(now);
    }
    else
    {
        return Encoding.UTF8.GetString(now);
    }
}
```