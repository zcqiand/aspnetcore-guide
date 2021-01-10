using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.WebApi
{
    public class TenantRequestHandler : IRequestHandler<TenantRequest, string>
    {
        private readonly ILogger<TenantRequestHandler> logger;
        public TenantRequestHandler(ILogger<TenantRequestHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<string> Handle(TenantRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Handled: {request.Message}");
            return Task.FromResult(request.Message);
        }
    }
}
