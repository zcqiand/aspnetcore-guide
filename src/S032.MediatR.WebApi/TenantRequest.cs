namespace MediatR.WebApi
{
    public class TenantRequest : IRequest<string>
    {
        public TenantRequest(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }
}