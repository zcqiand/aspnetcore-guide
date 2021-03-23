namespace DI.NoInjection.ConsoleApp
{
    public class Actor
    {
        private string name = "小明";
        public void Kill()
        {
            var knife = new Knife();
            knife.Kill(name);
        }
    }
}
