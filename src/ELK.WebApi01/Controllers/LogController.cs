using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ELK.WebApi01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogController : Controller
    {
        private readonly ILogger<LogController> logger; // <-添加此行
        public LogController(ILogger<LogController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public void Get()
        {
            logger.LogInformation("测试1"); // <-添加此行
        }
    }
}
