using System;
using System.Linq;
using System.Collections.Generic;

namespace DDD.ConsoleApp01
{
    class Program
    {
        public abstract class ValueObject
        {
            protected abstract IEnumerable<object> GetEqualityComponents();
            public override bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != GetType())
                {
                    return false;
                }

                var other = (ValueObject)obj;

                return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
            }
        }

        public class Address : ValueObject
        {
            public string Province;
            public string City;

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return Province;
                yield return City;
            }
        }

        static void Main(string[] args)
        {
            var xm = new Address { Province = "浙江", City = "宁波" };
            var xh = new Address { Province = "浙江", City = "宁波" };
            var xw = new Address { Province = "浙江", City = "杭州" };

            Console.WriteLine(xm.Equals(xh));
            Console.WriteLine(xm.Equals(xw));

            Console.ReadKey();
        }
    }
}
