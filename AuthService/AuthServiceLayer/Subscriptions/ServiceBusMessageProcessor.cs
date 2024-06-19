using AuthServiceLayer.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ServiceBusMessageProcessor
{
    private readonly string _connectionString;
    private readonly string _topicName;
    private readonly string _subscriptionName;
    private readonly ILogger<ServiceBusMessageProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;

    public ServiceBusMessageProcessor(IConfiguration configuration, ILogger<ServiceBusMessageProcessor> logger, IServiceScopeFactory scopeFactory)
    {
        _connectionString = configuration["ServiceBus:ConnectionString"];
        _topicName = configuration["ServiceBus:DeleteTopicName"];
        _subscriptionName = configuration["ServiceBus:DeleteSubscriptionName"];
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        _client = new ServiceBusClient(_connectionString);
        _processor = _client.CreateProcessor(_topicName, _subscriptionName, new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += ProcessMessageHandler;
        _processor.ProcessErrorAsync += ProcessErrorHandler;

        await _processor.StartProcessingAsync(cancellationToken);

        _logger.LogInformation("Service Bus message processing started.");
    }

    public async Task StopProcessingAsync()
    {
        if (_processor != null)
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }

        if (_client != null)
        {
            await _client.DisposeAsync();
        }

        _logger.LogInformation("Service Bus message processing stopped.");
    }

    private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
    {
        string message = args.Message.Body.ToString();
        try
        {
            if (message.StartsWith("User "))
            {
                // Find the index of the space after "User "
                int startIndex = message.IndexOf("User ") + "User ".Length;

                // Find the index of the next space after the user ID
                int endIndex = message.IndexOf(' ', startIndex);

                // Extract the substring that contains the user ID
                string userIdPart = message.Substring(startIndex, endIndex - startIndex);

                if (int.TryParse(userIdPart, out int userId))
                {
                    Console.WriteLine($"User ID extracted: {userId}");

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                        authService.DeleteUser(userId);
                    }
                }
                else
                {
                    Console.WriteLine("User ID extraction failed.");
                }
            }
            else
            {
                _logger.LogError("Unexpected message format for delete message.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing delete message.");
        }


        await args.CompleteMessageAsync(args.Message);
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, $"Error processing message: {args.Exception.Message}");

        // Add your error handling logic here

        return Task.CompletedTask;
    }
}
