using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace Quartz.ConsoleApp01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("开始调度!");

            //1、创建一个调度
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            //2、创建一个任务
            var job = JobBuilder.Create<HelloJob>()
                .WithIdentity("job1", "group1")
                .Build();

            //3、创建一个触发器
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithCronSchedule("0/5 * * * * ?")     //5秒执行一次
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            Console.ReadKey();
        }
    }
}
