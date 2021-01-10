using Microsoft.AspNetCore.Mvc;
using System;

namespace DI.Autofac.WebApi.Controllers
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
    }
}
