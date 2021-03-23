using Microsoft.AspNetCore.Mvc;

namespace MiddlewareWebApp07.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public void Get()
        {
        }
    }
}
