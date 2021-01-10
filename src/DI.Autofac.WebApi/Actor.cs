namespace DI.Autofac.WebApi
{
    public class Actor
    {
        private string name = "小明";
        private Knife knife;
        public Actor(Knife knife)
        {
            this.knife = knife;
        }

        public string Kill()
        {
            return knife.Kill(name);
        }
    }
}
