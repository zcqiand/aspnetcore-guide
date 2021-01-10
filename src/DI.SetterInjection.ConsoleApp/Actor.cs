namespace DI.SetterInjection.ConsoleApp
{
    public class Actor
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
