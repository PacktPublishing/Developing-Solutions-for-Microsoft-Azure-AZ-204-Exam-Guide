using System;
using System.Linq;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Encodings;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;
using System.Collections.Generic;

namespace consumer
{

    class Program
    {
        /*
        /  update connectionString value with output of previous script run. 
        */
        static string connectionString = "<your connection string from previous script run>";

        static string topicName = "booking";
        static string subscriptionName = "flight-booking";

        static async Task Main()
        {
            // configure client
            ServiceBusClient client = new ServiceBusClient(connectionString);

            // create a processor that we can use to process the messages
            await using ServiceBusProcessor processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

            // configure the message and error handler to use
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            Console.WriteLine($"Start listening topic '{topicName}' from '{subscriptionName}' subscription");

            async Task MessageHandler(ProcessMessageEventArgs args)
            {
                var booking = JsonSerializer.Deserialize<Booking>(args.Message.Body);

                //print only airbookings
                booking.AirBookings.ToList().ForEach(x => Console.WriteLine($"Flight: {x.To}=>{x.From} {x.FlighDate.Date}"));

                // we can evaluate application logic and use that to determine how to settle the message.
                await args.CompleteMessageAsync(args.Message);
            }

            Task ErrorHandler(ProcessErrorEventArgs args)
            {
                // the error source tells me at what point in the processing an error occurred
                Console.WriteLine(args.ErrorSource);
                // the fully qualified namespace is available
                Console.WriteLine(args.FullyQualifiedNamespace);
                // as well as the entity path
                Console.WriteLine(args.EntityPath);
                Console.WriteLine(args.Exception.ToString());
                return Task.CompletedTask;
            }

            // start processing
            await processor.StartProcessingAsync();

            Console.WriteLine("Press key to stop listening the topic");
            // since the processing happens in the background, we add a Console.ReadKey to allow the processing to continue until a key is pressed.
            Console.ReadKey();
        }


    }
}
