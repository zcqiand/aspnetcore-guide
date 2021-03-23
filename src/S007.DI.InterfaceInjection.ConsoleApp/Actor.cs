namespace DI.InterfaceInjection.ConsoleApp
{
    public class Actor: IActor
    {
        private string name = "小明";
        private Knife knife;
        public Knife Knife
        {
            set 
            {
                this.knife = value;
            }
            get
            {
                return this.knife;
            }
        }

        public void Kill()
        {
            knife.Kill(name);
        }
    }
}
