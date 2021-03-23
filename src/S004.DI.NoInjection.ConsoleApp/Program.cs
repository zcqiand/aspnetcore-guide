using System;

namespace DI.NoInjection.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var actor = new Actor();
            actor.Kill();

            Console.ReadKey();
        }
    }
}
