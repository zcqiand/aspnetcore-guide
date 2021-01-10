using Autofac;
using System;

namespace DI.Autofac.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Knife>();
            builder.RegisterType<Actor>();

            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var actor = scope.Resolve<Actor>();
                actor.Kill();
            }

            Console.ReadKey();
        }
    }
}
