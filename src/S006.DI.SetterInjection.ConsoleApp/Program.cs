using System;

namespace DI.SetterInjection.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var knife = new Knife();
            var actor = new Actor();
            actor.Knife = knife;
            actor.Kill();

            Console.ReadKey();
        }
    }
}
