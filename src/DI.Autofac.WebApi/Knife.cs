using System;

namespace DI.Autofac.WebApi
{
    public class Knife
    {
        public string Kill(string name)
        {
            return $"{name}用刀杀怪";
        }
    }
}
