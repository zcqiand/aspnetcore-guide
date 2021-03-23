using System;

namespace S003.DesignPattern.Mediator.ConsoleApp
{
    //1. 声明中介者接口并描述中介者和各种组件之间所需的交流接口。 
    public interface IMediator
    {
        void Notify(object sender, string ev);
    }

    //2. 实现具体中介者类。 
    class ConcreteMediator : IMediator
    {
        private readonly LandlordComponent landlordComponent;
        private readonly TenantComponent tenantComponent;

        public ConcreteMediator(LandlordComponent landlordComponent, TenantComponent tenantComponent)
        {
            this.landlordComponent = landlordComponent;
            this.landlordComponent.SetMediator(this);

            this.tenantComponent = tenantComponent;
            this.tenantComponent.SetMediator(this);
        }

        public void Notify(object sender, string ev)
        {
            if (ev == "求租")
            {
                Console.WriteLine("中介收到求租信息后通知房东。");
                landlordComponent.DoB();
            }
            if (ev == "出租")
            {
                Console.WriteLine("中介收到出租信息后通知房客。");
                tenantComponent.DoD();
            }
        }
    }

    //3. 组件基础类会使用中介者接口与中介者进行交互。
    class BaseComponent
    {
        protected IMediator mediator;
        public void SetMediator(IMediator mediator)
        {
            this.mediator = mediator;
        }
    }

    // 4. 具体组件房东，不与房客进行交流，只向中介者发送通知。
    class LandlordComponent : BaseComponent
    {
        public void DoA()
        {
            Console.WriteLine("房东有房子空出来了，向中介发送出租信息。");
            mediator.Notify(this, "出租");
        }
        public void DoB()
        {
            Console.WriteLine("房东收到求租信息，进行相应的处理。");
        }
    }

    // 具体组件房客，也只向中介者发送通知。
    class TenantComponent : BaseComponent
    {
        public void DoC()
        {
            Console.WriteLine("房客没有房子住了，向中介发送求租信息。");
            mediator.Notify(this, "求租");
        }
        public void DoD()
        {
            Console.WriteLine("房客收到出租信息，进行相应的处理。");
        }
    }

    // 客户端代码
    class Program
    {
        static void Main(string[] args)
        {
            LandlordComponent landlordComponent = new LandlordComponent();
            TenantComponent tenantComponent = new TenantComponent();
            new ConcreteMediator(landlordComponent, tenantComponent);

            landlordComponent.DoA();

            Console.WriteLine();

            tenantComponent.DoC();

            Console.ReadKey();
        }
    }
}
