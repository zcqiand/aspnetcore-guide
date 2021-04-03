using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace S046.Topshelf.ConsoleApp
{
    public class MyService
    {
        private readonly ILogger logger;
        private readonly AppSettings settings;

        public MyService(IOptions<AppSettings> settings, ILogger<MyService> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }
        public void Start()
        {
            logger.LogInformation($"Starting {this.settings.ServiceName}...");
        }

        public void Stop()
        {
            logger.LogInformation($"Stopping {this.settings.ServiceName}...");
        }
    }
}
