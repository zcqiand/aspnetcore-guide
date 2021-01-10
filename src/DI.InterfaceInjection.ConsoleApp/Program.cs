using System;

namespace DI.InterfaceInjection.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var knife = new Knife();
            IActor actor = new Actor();
            actor.Knife = knife;
            actor.Kill();

            Console.ReadKey();
        }
    }
}
