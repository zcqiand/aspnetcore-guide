using Serilog;
using System;
using Topshelf;

namespace S045.Topshelf.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            var rc = HostFactory.Run(x =>
            {
                x.UseSerilog(); // HostLogger改为使用Serilog。
                x.SetDisplayName("我的服务");  // 我们设置要在Windows服务监视器中使用的winservice的显示名称。
                x.SetDescription("我的服务详细描述"); // 我们设置了在Windows服务监视器中使用的winservice的描述。
                x.SetServiceName("MyService"); // 我们设置要在Windows服务监视器中使用的winservice的服务名称。
                x.Service<MyService>(s =>
                {
                    s.ConstructUsing(name => new MyService()); // 构建服务实例。
                    s.WhenStarted(tc => tc.Start()); // 启动服务
                    s.WhenStopped(tc => tc.Stop()); // 停止服务
                });
                x.RunAsLocalSystem(); // 设置“登录为”并选择了“本地系统”。
                x.StartAutomatically(); // 设置“启动类型”并选择了“自动”。
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode()); // 我们转换并返回服务退出代码。
            Environment.ExitCode = exitCode;
        }
    }
}
