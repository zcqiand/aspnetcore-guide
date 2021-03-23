using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Quartz.ConsoleApp02
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("开始调度!");

            //1、首先，我们必须获得对调度程序的引用
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = "XmlConfiguredInstance",
                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                ["quartz.threadPool.threadCount"] = "5",
                ["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
                ["quartz.plugin.xml.fileNames"] = "~/quartz_jobs.xml",
                // this is the default
                ["quartz.plugin.xml.FailOnFileNotFound"] = "true",
                // this is not the default
                ["quartz.plugin.xml.failOnSchedulingError"] = "true"
            };

            //2、创建一个调度
            var factory = new StdSchedulerFactory(properties);
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            Console.ReadKey();
        }
    }
}
