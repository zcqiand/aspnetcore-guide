namespace DI.ConstructorInjection.ConsoleApp
{
    public class Actor
    {
        private string name = "小明";
        private Knife knife;
        public Actor(Knife knife)
        {
            this.knife = knife;
        }

        public void Kill()
        {
            knife.Kill(name);
        }
    }
}
