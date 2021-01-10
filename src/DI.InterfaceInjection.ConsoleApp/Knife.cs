using System;

namespace DI.InterfaceInjection.ConsoleApp
{
    public class Knife
    {
        public void Kill(string name)
        {
            Console.WriteLine($"{name}用刀杀怪");
        }
    }
}
