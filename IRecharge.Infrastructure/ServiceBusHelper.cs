using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IRecharge.Infrastructure
{
    public class ServiceBusHelper
    {
        private readonly string _connectionString;

        public ServiceBusHelper(IConfiguration configuration)
        {
            _connectionString = configuration["AzureServiceBus:ConnectionString"];
        }

        // Publish an event to a specific topic
        public async Task PublishEventAsync(string topicName, object eventData)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(topicName);

            // Serialize event data to JSON
            string messageBody = JsonConvert.SerializeObject(eventData);
            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(message);
            Console.WriteLine($"Published event to {topicName}: {messageBody}");
        }

        // Subscribe to a topic and handle incoming messages using ProcessMessageEventArgs
        public async Task SubscribeToTopicAsync(string topicName, string subscriptionName, Func<ProcessMessageEventArgs, Task> messageHandler)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusProcessor processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 5,
                AutoCompleteMessages = false // Control message completion manually
            });

            processor.ProcessMessageAsync += async args =>
            {
                try
                {
                    await messageHandler(args);
                    await args.CompleteMessageAsync(args.Message); // Complete the message after processing
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message processing failed: {ex.Message}");
                    await args.AbandonMessageAsync(args.Message); // Abandon the message if processing fails
                }
            };

            processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine($"Error in message processing: {args.Exception.Message}");
                return Task.CompletedTask;
            };

            // Start processing messages
            await processor.StartProcessingAsync();
            Console.WriteLine($"Started receiving messages from topic: {topicName}, subscription: {subscriptionName}");
        }
    }


}
