using System;
using System.Collections.Generic;
using System.Threading;

namespace DesignPattern.Observer.ConsoleApp01
{
    //声明订阅者接口。
    public interface IObserver
    {
        // 通知后处理
        void Handle(ISubject subject);
    }

    //声明发布者接口并定义一些接口来在列表中添加和删除订阅对象。 
    public interface ISubject
    {
        // 订阅
        void Subscribe(IObserver observer);

        // 取消订阅
        void Unsubscribe(IObserver observer);

        // 发布
        void Publish();
    }

    //创建具体发布者类。
    public class Subject : ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();

        public void Subscribe(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Publish()
        {
            Console.WriteLine("商店发布优惠通知！");
            foreach (var observer in _observers)
            {
                observer.Handle(this);
            }
        }
    }

    //具体订阅者类中实现通知后处理的方法。
    public class CustomerA : IObserver
    {
        public void Handle(ISubject subject)
        {
            Console.WriteLine("顾客A收到优惠通知。");
        }
    }

    public class CustomerB : IObserver
    {
        public void Handle(ISubject subject)
        {
            Console.WriteLine("顾客B收到优惠通知。");
        }
    }

    //客户端必须生成所需的全部订阅者， 并在相应的发布者处完成注册工作。
    class Program
    {
        static void Main(string[] args)
        {
            var subject = new Subject();

            var observerA = new CustomerA();
            subject.Subscribe(observerA);
            var observerB = new CustomerB();
            subject.Subscribe(observerB);
            subject.Publish();

            Console.WriteLine();

            subject.Unsubscribe(observerB);
            subject.Publish();

            Console.ReadKey();
        }
    }
}
