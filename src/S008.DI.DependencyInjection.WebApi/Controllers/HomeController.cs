using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DI.DependencyInjection.WebApi01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {

        private readonly Actor actor;
        public HomeController(Actor actor)
        {
            this.actor = actor ?? throw new ArgumentNullException(nameof(actor));
        }

        [HttpGet]
        public string Get()
        {
            return actor.Kill();
        }

        [Route("ServiceLifetime")]
        [HttpGet]
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
