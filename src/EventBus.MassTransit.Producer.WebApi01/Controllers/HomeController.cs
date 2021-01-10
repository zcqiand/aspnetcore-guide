using EventBus.MassTransit.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventBus.MassTransit.Producer.WebApi01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;
        public HomeController(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            await publishEndpoint.Publish<SendedEvent>(new SendedEvent("优惠"));
            return Ok();
        }
    }
}
