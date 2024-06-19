namespace AuthServiceLayer.Subscriptions
{
    public class ServiceBusHostedService : IHostedService
    {
        private readonly ServiceBusMessageProcessor _messageProcessor;
        private readonly RegisterProcessor _registerProcessor;
        private readonly ILogger<ServiceBusHostedService> _logger;

        public ServiceBusHostedService(ServiceBusMessageProcessor messageProcessor, RegisterProcessor registerProcessor, ILogger<ServiceBusHostedService> logger)
        {
            _messageProcessor = messageProcessor;
            _registerProcessor = registerProcessor;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Azure Service Bus message processing...");
            await _registerProcessor.StartProcessingAsync(cancellationToken);
            await _messageProcessor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Azure Service Bus message processing...");
            await _registerProcessor.StopProcessingAsync();
            await _messageProcessor.StopProcessingAsync();
        }
    }

}
