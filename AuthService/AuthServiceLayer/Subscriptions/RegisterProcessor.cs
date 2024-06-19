using AuthServiceLayer.Models;
﻿using AuthServiceLayer.Services;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace AuthServiceLayer.Subscriptions
{
    public class RegisterProcessor
    {
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly ILogger<RegisterProcessor> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private ServiceBusClient _client;
        private ServiceBusProcessor _processor;

        public RegisterProcessor(IConfiguration configuration, ILogger<RegisterProcessor> logger, IServiceScopeFactory scopeFactory)
        {
            _connectionString = configuration["ServiceBus:ConnectionString"];
            _topicName = configuration["ServiceBus:RegisterTopicName"];
            _subscriptionName = configuration["ServiceBus:RegisterSubscriptionName"];
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

            _logger.LogInformation("Register message processing started.");
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

            _logger.LogInformation("Register message processing stopped.");
        }

        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            string messageBody = args.Message.Body.ToString();

            try
            {
                var message = JsonSerializer.Deserialize<UserMessage>(messageBody);

                if (message != null)
                {
                    await HandleRegisterUser(message);
                }
                else
                {
                    _logger.LogError("Failed to deserialize register message body.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing register message.");
            }

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, $"Error processing message: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private async Task HandleRegisterUser(UserMessage message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                User user = new User { UserId = message.Id, Email = message.Email, Password = message.Password, Role = message.Role };
                authService.Register(user);
            }
        }
    }
}
