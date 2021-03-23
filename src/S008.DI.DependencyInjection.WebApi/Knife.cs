using System;

namespace DI.DependencyInjection.WebApi01
{
    public class Knife
    {
        public string Kill(string name)
        {
            return $"{name}用刀杀怪";
        }
    }
}
