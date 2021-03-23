namespace MediatR.WebApi
{
    public class LandlordNotification : INotification
    {
        public LandlordNotification(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }
}