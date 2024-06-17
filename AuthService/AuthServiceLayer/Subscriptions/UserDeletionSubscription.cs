using Azure.Messaging.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using AuthServiceLayer.Services;

namespace AuthServiceLayer.Subscriptions
{
    public class UserDeletionSubscription
    {
        private readonly string serviceBusConnectionString = "Endpoint=sb://bus-articulation.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=trv17NvCsHB8Y+YFDhccpYbW1xxo3x44M+ASbGkhr1A=";
        private readonly string topicName = "userdeletiontopic"; 
        private readonly string subscriptionName = "DeleteAuthData";
        private readonly IAuthService _authService;

        public UserDeletionSubscription(IAuthService authService)
        {
            this._authService = authService;
        }

        public async Task StartListeningAsync()
        {
            await using var client = new ServiceBusClient(serviceBusConnectionString);
            var processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += ProcessMessageHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            await processor.StartProcessingAsync();
        }

        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            string userId = args.Message.Body.ToString();
            Console.WriteLine($"Received message: {userId}");

            try
            {
                await DeleteUserCredentialsAsync(userId);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                // Optionally abandon the message in case of an error.
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Message handler encountered an exception {args.Exception}.");
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Entity Path: {args.EntityPath}");
            Console.WriteLine($"- Executing Action: {args.ErrorSource}");
            return Task.CompletedTask;
        }

        private Task DeleteUserCredentialsAsync(string userId)
        {
            Console.WriteLine($"Deleting credentials for user_id: {userId}");
            _authService.DeleteUser(userId);

            return Task.CompletedTask;
        }
    }

    public class UserDeletionBackgroundService : BackgroundService
    {
        private readonly UserDeletionSubscription _userDeletionSubscription;

        public UserDeletionBackgroundService(UserDeletionSubscription userDeletionSubscription)
        {
            _userDeletionSubscription = userDeletionSubscription;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _userDeletionSubscription.StartListeningAsync();
        }
    }
}
