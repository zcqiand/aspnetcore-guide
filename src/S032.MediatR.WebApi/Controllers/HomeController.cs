using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MediatR.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IMediator mediator;
        public HomeController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Route("Example01")]
        [HttpGet]
        public async Task<IActionResult> Example01()
        {
            await mediator.Send(new TenantRequest("通知房客信息"));
            return Ok();
        }

        [Route("Example02")]
        [HttpGet]
        public async Task<IActionResult> Example02()
        {
            await mediator.Publish(new LandlordNotification("通知房东信息"));
            return Ok();
        }
    }
}
