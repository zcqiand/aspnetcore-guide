using System;

namespace HelloConsoleApp01
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Hello World!");
            }
            else
            {
                Console.WriteLine($"Hello {string.Join(' ', args)}!");
            }
        }
    }
}
