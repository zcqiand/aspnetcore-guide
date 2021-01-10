using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Cache.WebApi01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CacheController : Controller
    {
        private IMemoryCache cache;
        public CacheController(IMemoryCache cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

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
    }
}
