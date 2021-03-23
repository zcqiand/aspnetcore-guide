using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIWebApp03.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private Knife knife;
        public HomeController(Knife knife)
        {
            this.knife = knife;
        }

        [HttpPost]
        [Route("Kill01")]
        public string Kill01()
        {
            return knife.Kill("小明");
        }

        [HttpPost]
        [Route("Kill02")]
        public string Kill02([FromServices] Knife knife2)
        {
            return knife2.Kill("小明");
        }

        [HttpGet]
        [Route("ServiceLifetime")]
        public List<string> GetServiceLifetime([FromServices] MySingletonService singleton1,
        [FromServices] MySingletonService singleton2,
        [FromServices] MyScopedService scoped1,
        [FromServices] MyScopedService scoped2,
        [FromServices] MyTransientService transient1,
        [FromServices] MyTransientService transient2)
        {
            var s = new List<string>();
            s.Add($"singleton1:{singleton1.GetHashCode()}");
            s.Add($"singleton2:{singleton2.GetHashCode()}");
            s.Add($"scoped1:{scoped1.GetHashCode()}");
            s.Add($"scoped2:{scoped2.GetHashCode()}");
            s.Add($"transient1:{transient1.GetHashCode()}");
            s.Add($"transient2:{transient2.GetHashCode()}");
            return s;
        }
    }
}
