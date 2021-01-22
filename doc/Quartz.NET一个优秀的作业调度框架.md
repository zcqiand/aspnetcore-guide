# 1 什么是Quartz.NET？
Docker是一个功能齐全的开源作业调度系统，可以与几乎任何其他软件系统集成或一起使用。

# 2 为什么需要Quartz.NET？
.NET Framework通过System.Timers.Timer类具有“内置”计时器功能-为什么有人使用Quartz而不是这些标准功能？

原因有很多！这里有一些：

* 计时器没有持久性机制。
* 计时器的时间安排不灵活（只能设置开始时间和重复间隔，没有基于日期，一天中的时间等信息）。
* 计时器不使用线程池（每个计时器一个线程）
* 计时器没有真正的管理方案-您必须编写自己的机制以能够按名称记住，组织和恢复任务等。

# 3 如何使用Quartz.NET？
**首先**，安装Quartz
```
Quartz
```

**然后**，定义一个任务类
```
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
```


**最后**，实例化并启动调度程序，并调度要执行的作业：
```
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
```

我们来看一下输出结果：
```
开始调度!
Hello 2021/1/19 14:37:40
Hello 2021/1/19 14:37:45
Hello 2021/1/19 14:37:50
Hello 2021/1/19 14:37:55
```

# 3 使用配置文件方式使用Quartz.NET？
**首先**，安装Quartz，Quartz.Plugins
```
Quartz
Quartz.Plugins
```

**然后**，定义一个任务类
```
using System;
using System.Threading.Tasks;

namespace Quartz.ConsoleApp02
{
    public class HelloJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello " + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
```

**接着**，配置quartz_jobs.xml文件
```
<?xml version="1.0" encoding="UTF-8"?>
<!-- This file contains job definitions in schema version 2.0 format -->
<job-scheduling-data xmlns="http://quartznet.sourceforge.net/JobSchedulingData" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" version="2.0">
	<processing-directives>
		<overwrite-existing-data>true</overwrite-existing-data>
	</processing-directives>
	<schedule>
		<!--TestJob测试 任务配置-->
		<job>
			<name>job1</name>
			<group>group1</group>
			<description>job1</description>
			<job-type>Quartz.ConsoleApp02.HelloJob,Quartz.ConsoleApp02</job-type>
			<durable>true</durable>
			<recover>false</recover>
		</job>
		<trigger>
			<cron>
				<name>trigger1</name>
				<group>group1</group>
				<job-name>job1</job-name>
				<job-group>group1</job-group>
				<cron-expression>0/5 * * * * ?</cron-expression>
			</cron>
		</trigger>
	</schedule>
</job-scheduling-data>
```

![](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210115/082334327217822.png)


**最后**，实例化并启动调度程序：
```
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
```

# 4 Cron表达式
## 4.1 介绍
cron是已存在很长时间的UNIX工具，因此其调度功能强大且经过验证。
cron表达式是由7段构成：秒 分 时 日 月 星期 年（可选）

* "*" 用于选择字段中的所有值，例如，在分钟字段中表示“每分钟”。
* "?" 在需要在允许使用字符的两个字段之一中指定某些内容而在另一个不允许使用的字段中指定内容时很有用。例如，如果我希望触发器在每月的某个特定日期（例如10号）触发，但不在乎是星期几，则将其10输入“月日”字段，以及?“星期几”字段中。
* "-" 用于指定范围。例如，10-12在小时字段中表示“小时10、11和12”。
* "," 用于指定其他值。例如，MON,WED,FRI在“星期几”字段中表示“星期一，星期三和星期五的日子”。
* "/" 用于指定增量。例如，0/15在秒字段中表示“秒0、15、30和45”。和5/15在秒字段的意思是“秒5，20，35和50”。
* "L" 在允许使用的两个字段中都有不同的含义。例如，“L月日”字段中的值表示“月的最后一天”
* "W" 用于指定最接近给定日期的工作日（星期一至星期五）。例如，如果您要指定“15W月日”字段的值，则含义是：“离月15日最近的工作日”。
* "#" 用于指定每月的“第n个” XXX天。例如，“6#3星期几”字段中的值表示“每月的第三个星期五”（第6天=星期五，“＃3” =每月的第三个星期五）。

## 4.2 例子
|表达|含义|
|-|-|
|0 0 12 * * ?|每天中午12点（中午）触发|
|0 15 10 ? * *|每天上午10:15触发|
|0 15 10 * * ?|每天上午10:15触发|
|0 15 10 * * ? *|每天上午10:15触发|
|0 15 10 * * ? 2005|2005年期间，每天上午10:15触发|
|0 * 14 * * ?|每天从下午2点开始，直到下午2:59结束，每分钟触发一次|
|0 0/5 14 * * ?|每天从下午2点开始，直到下午2:55，每5分钟触发一次|
|0 0/5 14,18 * * ?|每天从下午2点开始到下午2:55结束，每5分钟触发一次，并且每天下午6点开始到下午6:55结束，每5分钟触发一次|
|0 0-5 14 * * ?|每天从下午2点开始，直到下午2:05结束，每分钟触发一次|
|0 10,44 14 ? 3 WED|3月的每个星期三下午2:10和2:44 pm触发。|
|0 15 10 ? * MON-FRI|每个星期一，星期二，星期三，星期四和星期五的上午10:15触发|
|0 15 10 15 * ?|每个月的15日上午10:15触发|
|0 15 10 L * ?|每个月的最后一天上午10:15触发|
|0 15 10 L-2 * ?|每个月的倒数第二个上午10:15触发|
|0 15 10 ? * 6L|每个月的最后一个星期五上午10:15触发|
|0 15 10 ? * 6L|每个月的最后一个星期五上午10:15触发|
|0 15 10 ? * 6L 2002-2005|在2002、2003、2004和2005年的每个月的最后一个星期五上午10:15触发|
|0 15 10 ? * 6#3|每个月的第三个星期五上午10:15触发|
|0 0 12 1/5 * ?|从每月的第一天开始，每月每5天在中午12点（中午）触发。|
|0 11 11 11 11 ?|每年11月11日上午11:11触发。|