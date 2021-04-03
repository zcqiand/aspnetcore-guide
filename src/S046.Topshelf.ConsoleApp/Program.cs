using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using Topshelf;

namespace S046.Topshelf.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogging();
            var host = CreateHostBuilder(args).Build();
            RunWindowsServiceWithHost(host);
        }

        private static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<AppSettings>(hostContext.Configuration);
                    services.AddTransient<MyService>();
                });

        private static void RunWindowsServiceWithHost(IHost host)
        {
            var rc = HostFactory.Run(x =>
            {
                x.UseSerilog();
                x.SetDisplayName("我的服务");
                x.SetDescription("我的服务详细描述");
                x.SetServiceName("MyService");

                var myService = host.Services.GetRequiredService<MyService>();
                x.Service<MyService>(s =>
                {
                    s.ConstructUsing(() => myService);
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
