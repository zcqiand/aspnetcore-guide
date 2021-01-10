using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.WebApi
{
    public class LandlordNotificationHandler : INotificationHandler<LandlordNotification>
    {
        private readonly ILogger<LandlordNotificationHandler> logger;
        public LandlordNotificationHandler(ILogger<LandlordNotificationHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(LandlordNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Handled: {notification.Message}");
            return Task.CompletedTask;
        }
    }
}