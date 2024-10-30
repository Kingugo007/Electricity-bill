using Azure.Messaging.ServiceBus;
using IRecharge.Core.Application.BillingServices.BillDto;
using IRecharge.Core.Application.Interface;
using IRecharge.Core.Application.WalletServices;
using IRecharge.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IRecharge.Core.BackgroundServices
{
    public class WalletServiceBusBackgroundService : BackgroundService
    {
        private ServiceBusClient client;
        private ServiceBusProcessor processor;

        private readonly string connectionStringServiceBus = string.Empty;
        private readonly ILogger<WalletServiceBusBackgroundService> _logger;
        private readonly IWalletService _walletService;
        private readonly INotificationService _notificationService;

        private readonly string usertopicName = "user-events";
        private readonly string walletSubscriptionName = "create-wallet";
        private readonly string paymentTopicName = "payment-completed";
        private readonly string notificationSubName = "sms-notification";


        public WalletServiceBusBackgroundService(IConfiguration configuration, ILogger<WalletServiceBusBackgroundService> logger, IServiceProvider serviceProvider)
        {
            connectionStringServiceBus = configuration["AzureServiceBus:ConnectionString"];
            _logger = logger;
            _walletService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IWalletService>();
            _notificationService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<INotificationService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               await CreateWalletProcessor(stoppingToken);
               await NotificationProcessor(stoppingToken);
            }
        }

        private async Task CreateWalletProcessor(CancellationToken stoppingToken)
        {
            client = new ServiceBusClient(connectionStringServiceBus);
            processor = client.CreateProcessor(usertopicName, walletSubscriptionName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false
            });

            processor.ProcessMessageAsync += ProcessCreateWalletMessagesAsync;
            processor.ProcessErrorAsync += ExceptionReceivedHandler;

            await processor.StartProcessingAsync(stoppingToken);

            // Wait until the application is shutting down
            stoppingToken.Register(() => processor.StopProcessingAsync().GetAwaiter().GetResult());
            await Task.Delay(100000, stoppingToken);
        }
        private async Task ProcessCreateWalletMessagesAsync(ProcessMessageEventArgs args)
        {
            // Process the message
            string jsonString = args.Message.Body.ToString();

            _logger.LogInformation($"WalletServiceBus: {jsonString}");

            var user = JsonConvert.DeserializeObject<AppUser>(jsonString);

            CreateWallet newWallet = new CreateWallet()
            {
                PhoneNumber = user.PhoneNumber,
                UserId = user.Id
            };

            var wallet = await _walletService.CreateWallet(newWallet);
            // Complete the message so that it is not received again.
            await args.CompleteMessageAsync(args.Message);
        }      

        private async Task NotificationProcessor(CancellationToken stoppingToken)
        {
            client = new ServiceBusClient(connectionStringServiceBus);
            processor = client.CreateProcessor(paymentTopicName, notificationSubName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false
            });

            processor.ProcessMessageAsync += NotificationMessagesAsync;
            processor.ProcessErrorAsync += ExceptionReceivedHandler;

            await processor.StartProcessingAsync(stoppingToken);

            // Wait until the application is shutting down
            stoppingToken.Register(() => processor.StopProcessingAsync().GetAwaiter().GetResult());
            await Task.Delay(100000, stoppingToken);
        }
        private async Task NotificationMessagesAsync(ProcessMessageEventArgs args)
        {
            // Process the message
            string jsonString = args.Message.Body.ToString();

            _logger.LogInformation($"WalletServiceBus: {jsonString}");

            var response = JsonConvert.DeserializeObject<PaymentCompletedEvent>(jsonString);

            if(response != null)
            {
                await _notificationService.SendToken(response.UserId, response.Token);
                // Complete the message so that it is not received again.
                await args.CompleteMessageAsync(args.Message);
            }            
        }

        private Task ExceptionReceivedHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError($"WalletServiceBus {args.Exception}");
            return Task.CompletedTask;
        }
    }
}
