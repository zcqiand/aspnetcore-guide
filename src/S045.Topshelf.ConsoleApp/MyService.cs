using Serilog;
using System.Reflection;

namespace S045.Topshelf.ConsoleApp
{
    public class MyService
    {
        readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        public void Start()
        {
            log.Information("Starting MyService...");
        }

        public void Stop()
        {
            log.Information("Stopping MyService...");
        }
    }
}
