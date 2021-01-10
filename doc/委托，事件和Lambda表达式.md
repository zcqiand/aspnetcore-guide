# 1 委托
## 1.2 委托是什么？
委托是一种引用类型（其实就是一个类，继承MulticastDelegate特殊的类。），表示对具有特定参数列表和返回类型的方法的引用。 

每个委托提供Invoke方法, BeginInvoke和EndInvoke异步方法

## 1.3 为什么需要委托？
* 委托可以将方法（即逻辑）作为参数；
    * 逻辑解耦，保持稳定。
    * 代码复用，保证项目规范。

## 1.4 如何使用委托？
声明委托

    delegate void Del(string str);
    static void Notify(string name)
    {
        Console.WriteLine($"Notification received for: {name}");
    }
    
实例化委托

    Del del1 = new Del(Notify);
    //C# 2.0
    Del del2 = Notify;
    
调用委托

    del1.Invoke("小明");
    del2("小明");

其他使用委托

    //C# 2.0使用匿名方法来声明和实例化委托
    Del del3 = delegate(string name)
    { Console.WriteLine($"Notification received for: {name}"); };
    //C# 3.0使用lambda表达式声明和实例化委托
    Del del4 = name =>  { Console.WriteLine($"Notification received for: {name}"); };

简化开发过程，.NET 包含一组委托类型：
* Action<> 具有参数且不返回值。
* Func<> 具有参数且返回由参数指定的类型的值。
* Predicate<> 用于确定参数是否满足委托条件的情况。 
    
## 1.5 实际案例
代码：

    class Program
    {
        /// <summary>
        /// 声明委托
        /// </summary>
        /// <param name="fullName"></param>
        private delegate void KillDelegate(string fullName);
        static void Main(string[] args)
        {
            //实例化委托
            var killWithKnifeDelegate = new KillDelegate(KillWithKnife);
            Kill("郭靖", killWithKnifeDelegate);

            var killWithSwordDelegate = new KillDelegate(KillWithSword);
            Kill("黄蓉", killWithSwordDelegate);

            var killWithAxeDelegate = new KillDelegate(KillWithAxe);
            Kill("欧阳克", killWithAxeDelegate);

            Console.ReadKey();
        }

        static void Kill(string fullName, KillDelegate killDelegate)
        {
            Console.WriteLine($"{fullName}遇到怪物");
            //调用委托
            killDelegate.Invoke(fullName);
            Console.WriteLine($"{fullName}增长10经验");
        }

        static void KillWithKnife(string fullName)
        {
            Console.WriteLine($"{fullName}用刀杀怪物");
        }
        static void KillWithSword(string fullName)
        {
            Console.WriteLine($"{fullName}用剑杀怪物");
        }
        static void KillWithAxe(string fullName)
        {
            Console.WriteLine($"{fullName}用斧杀怪物");
        }
    }

# 2 Lambda表达式
## 2.1 Lambda是什么？
Lambda就是使用委托的更方便的语法。 

    //C# 2.0使用匿名方法来声明和实例化委托
    Del del3 = delegate(string name)
    { Console.WriteLine($"Notification received for: {name}"); };
    //C# 3.0使用lambda表达式声明和实例化委托
    Del del4 = name =>  { Console.WriteLine($"Notification received for: {name}"); };

## 2.2 为什么需要Lambda？
简化开发过程，并不会影响运行性能。

## 2.3 如何使用Lambda？
表达式lambda基本形式：

    //仅当 lambda 只有一个输入参数时，括号才是可选的；否则括号是必需的
    (input-parameters) => expression
    
使用空括号指定零个输入参数：

    Action line = () => Console.WriteLine();
    
括号内的两个或更多输入参数使用逗号加以分隔：

    Func<int, int, bool> testForEquality = (x, y) => x == y;
    
语句lambda

    (input-parameters) => { <sequence-of-statements> }
   
语句 lambda 的主体可以包含任意数量的语句；

    Action<string> greet = name =>
    {
        string greeting = $"Hello {name}!";
        Console.WriteLine(greeting);
    };
    greet("World");
    // Output:
    // Hello World!

使用匿名委托和lambda代码:

    public static void Main(string[] args)
    {
        List<int> list = new List<int>();
        for (int i = 1; i <= 100; i++)
        {
            list.Add(i);
        }

        //使用匿名委托
        List<int> result = list.FindAll(
          delegate (int no)
          {
              return (no % 2 == 0);
          }
        );
        foreach (var item in result)
        {
            Console.WriteLine(item);
        }
        
        //使用Lambda
        List<int> result = list.FindAll(i => i % 2 == 0);
        foreach (var item in result)
        {
            Console.WriteLine(item);
        }
    }    

# 3 事件
## 3.1 事件是什么？
事件是一种特殊的委托类型，主要用于消息或通知的传递。事件只能从事件的发布类型中调用，并且通常基于EventHandler委托，该委托具有代表事件发送者的对象和System.EventArgs派生的类，其中包含有关事件的数据。

## 3.2 何时使用委托和事件？
* 侦听事件是可选的:如果你的代码必须调用由订阅服务器提供的代码，则应使用基于委托的设计。如果你的代码在不调用任何订阅服务器的情况下可完成其所有工作，则应使用基于事件的设计。
* 返回值需要委托:用于事件的委托均具有无效的返回类型,事件处理程序通过修改事件参数对象的属性将信息传回到事件源。
* 事件具有专用调用:包含事件的类以外的类只能添加和删除事件侦听器；只有包含事件的类才能调用事件。

## 3.3 如何使用事件？
### 3.3.1 发布事件
定义事件数据

    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string message)
        {
            Message = message;
        }
    
        public string Message { get; set; }
    }

声明发布类中的事件

    public delegate void CustomEventHandler(object sender, CustomEventArgs args);
    public event CustomEventHandler RaiseCustomEvent;

    //使用泛型版本
    public event EventHandler<CustomEventArgs> RaiseCustomEvent;

### 3.3.2 订阅事件
定义一个事件处理程序方法

    void HandleCustomEvent(object sender, CustomEventArgs a)  
    {  
       // Do something useful here.  
    } 
    
使用(+=) 添加订阅事件

    publisher.RaiseCustomEvent += HandleCustomEvent;  

使用(-=) 取消订阅事件

    publisher.RaiseCustomEvent -= HandleCustomEvent;  
    
### 3.3.3 示例

    using System;
    namespace DotNetEvents
    {
        // 定义事件信息的类
        public class CustomEventArgs : EventArgs
        {
            public CustomEventArgs(string message)
            {
                Message = message;
            }
            public string Message { get; set; }
        }
        // 发布事件的类
        class Publisher
        {
            // 使用EventHandler <T>声明事件
            public event EventHandler<CustomEventArgs> RaiseCustomEvent;
            public void DoSomething()
            {
                RaiseCustomEvent(new CustomEventArgs("Event triggered"));
            }
        }
        //订阅事件的类
        class Subscriber
        {
            private readonly string _id;
            public Subscriber(string id, Publisher pub)
            {
                _id = id;
                // 添加订阅事件
                pub.RaiseCustomEvent += HandleCustomEvent;
            }
            // 定义一个事件处理程序方法。
            void HandleCustomEvent(object sender, CustomEventArgs e)
            {
                Console.WriteLine($"{_id} received this message: {e.Message}");
            }
        }
        class Program
        {
            static void Main()
            {
                var pub = new Publisher();
                var sub1 = new Subscriber("sub1", pub);
                var sub2 = new Subscriber("sub2", pub);
                // 调用引发事件的方法
                pub.DoSomething();
                Console.ReadKey();
            }
        }
    }

# 4 参考
* 委托 https://docs.microsoft.com/zh-cn/dotnet/csharp/delegates-overview
* Lambda表达式 https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/lambda-expressions
* 事件 https://docs.microsoft.com/zh-cn/dotnet/csharp/events-overview