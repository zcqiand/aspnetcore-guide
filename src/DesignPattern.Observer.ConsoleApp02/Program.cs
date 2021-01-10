using System;

namespace DesignPattern.Observer.ConsoleApp02
{
    //创建具体发布者类。
    public class Subject
    {
        public event Action Handles;

        public void Publish()
        {
            Console.WriteLine("商店发布优惠通知！");
            Handles?.Invoke();
        }
    }

    //具体订阅者类中实现通知后处理的方法。
    public class CustomerA
    {
        public void Handle()
        {
            Console.WriteLine("顾客A收到优惠通知。");
        }
    }
    public class CustomerB
    {
        public void Handle()
        {
            Console.WriteLine("顾客B收到优惠通知。");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var subject = new Subject();

            var observerA = new CustomerA();
            subject.Handles += observerA.Handle;
            var observerB = new CustomerB();
            subject.Handles += observerB.Handle;
            subject.Publish();

            Console.WriteLine();

            subject.Handles -= observerB.Handle;
            subject.Publish();

            Console.ReadKey();
        }
    }
}
