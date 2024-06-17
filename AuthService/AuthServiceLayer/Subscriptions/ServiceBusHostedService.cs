namespace AuthServiceLayer.Subscriptions
{
    public class ServiceBusHostedService : IHostedService
    {
        private readonly ServiceBusMessageProcessor _messageProcessor;
        private readonly ILogger<ServiceBusHostedService> _logger;

        public ServiceBusHostedService(ServiceBusMessageProcessor messageProcessor, ILogger<ServiceBusHostedService> logger)
        {
            _messageProcessor = messageProcessor;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Azure Service Bus message processing...");
            return _messageProcessor.StartProcessingAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Azure Service Bus message processing...");
            return _messageProcessor.StopProcessingAsync();
        }
    }

}
