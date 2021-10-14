using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        //private const string connectionString = "Endpoint=sb://fdass-perf.servicebus.windows.net/;SharedAccessKeyName=ReadWrite;SharedAccessKey=T8SOtgUJOp2rO/G8mYR9VWh5LMA6Zb1u6htOl9CmPKk=";

        //private const string eventHubName = "hackathon";

        //private const string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=fdaasperfehcheckpoints;AccountKey=A8UsjTFC9VMsh2W84vQHwK2XsjLlR7GwrgNQ5+oHftUV5Q5Eo4cp9uxNQjL802F4XTnHTruxgRKIIkpMVR9c4w==;EndpointSuffix=core.windows.net";

        //private const string blobContainerName = "hackathon";
        private const string connectionString = "Endpoint=http://localhost/;SharedAccessKeyName=DummyAccessKeyName;SharedAccessKey=5dOntTRytoC24opYThisAsit3is2B+OGY1US/fuL3ly="; 
        private const string eventHubName = "hackathon";



        static async Task Main()
        {
            Console.WriteLine("Hello");
            // Create a producer client that you can use to send events to an event hub
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {

                //Thread.Sleep(10000);
                //sleep ----------------------
                // Create a batch of events 
                Console.WriteLine("Hello1");

                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                //sleep-------------------------
                Console.WriteLine("Hello2");

                // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("A")));
                //eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Second event")));
                //eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Third event")));

                // Use the producer client to send the batch of events to the event hub
                Console.WriteLine("going to sleep");
                Thread.Sleep(10000);
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("A batch of 3 events has been published.");
            
            
            }

            /*
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Create a blob container client that the event processor will use 
            BlobContainerClient storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Create an event processor client to process events in the event hub
             var processor = new EventProcessorClient(storageClient, consumerGroup, connectionString, eventHubName);

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            await processor.StartProcessingAsync();

            // Wait for 10 seconds for the events to be processed
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Stop the processing
            await processor.StopProcessingAsync();
            */
            
        }
        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Write the body of the event to the console window
            Console.WriteLine("\tRecevied event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
