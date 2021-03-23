using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text;

namespace Cache.WebApi02.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CacheController : Controller
    {
        private IDistributedCache cache;
        public CacheController(IDistributedCache cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

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
    }
}
