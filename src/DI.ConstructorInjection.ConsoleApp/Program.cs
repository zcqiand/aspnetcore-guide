using System;

namespace DI.ConstructorInjection.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var knife = new Knife();
            var actor = new Actor(knife);
            actor.Kill();

            Console.ReadKey();
        }
    }
}
