using System;
using System.Threading.Tasks;

namespace Quartz.ConsoleApp01
{
    public class HelloJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello "+DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
