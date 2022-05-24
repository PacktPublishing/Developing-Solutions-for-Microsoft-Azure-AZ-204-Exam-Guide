using System;
using System.Linq;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Encodings;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;

namespace consumer
{
    class Program
    {
        /*
        /  update connectionString value with output of previous script run. 
        */
        static string connectionString = "<your connection string from previous script run>";
       
        static string queueName = "simple-queue";

        static async Task Main(string[] args)
        {            
            // configure admin client
            ServiceBusAdministrationClient adminClint = new ServiceBusAdministrationClient(connectionString);
            // configure client
            ServiceBusClient client = new ServiceBusClient(connectionString);


            while (true)
            {
                QueueRuntimeProperties runtimeProp = await adminClint.GetQueueRuntimePropertiesAsync(queueName);


                Console.WriteLine($"\r\nCurrent messages count: { runtimeProp.ActiveMessageCount }");
                Console.WriteLine("\r\nChose [1-3] for demonstration:");
                Console.WriteLine("\t1 - Receive with PeakLock ");
                Console.WriteLine("\t2 - Receive and delete");
                Console.WriteLine("\t3 - Batch Peek messages");
                Console.WriteLine("Chose [1-3]");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        await ReceivePickAndLockAsync(client);
                        break;

                    case ConsoleKey.D2:
                        await ReceiveAndDeleteAsync(client);
                        break;

                    case ConsoleKey.D3:
                        await PeekAsync(client, 32);
                        break;
                }
            }
        }

        /// <summary>
        /// The function will list all messages in the queue and keep them in
        /// </summary>
        /// <param name="client"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static async Task PeekAsync(ServiceBusClient client, int count)
        {
            //create a receiver
            ServiceBusReceiver receiver = client.CreateReceiver(queueName);

            //peak a message to keep in queue
            var messages =  await receiver.PeekMessagesAsync(count);

            foreach(var msg in messages)
            {
                var booking = JsonSerializer.Deserialize<Booking>(msg.Body);
                Console.WriteLine($"Message {msg.MessageId} peeked:\r\n{booking}");
            }

            //release resources
            await receiver.CloseAsync();
        }

        /// <summary>
        /// The function will demonstrate use of the auto delete message receiving
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task ReceiveAndDeleteAsync(ServiceBusClient client)
        {
            //configure receiver with auto delete message. 
            ServiceBusReceiver receiver = client.CreateReceiver(queueName,
                new ServiceBusReceiverOptions()
                { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

            // the received message is a different type as it contains some service set properties
            ServiceBusReceivedMessage msg = await receiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(3));

            if (msg != null)
            {
                // processing message....
                var booking = JsonSerializer.Deserialize<Booking>(msg.Body);
                Console.WriteLine($"Message {msg.MessageId} received:\r\n{booking}");
            }

            //release resources
            await receiver.CloseAsync();
        }


        /// <summary>
        /// The function will demonstrate use of the PeekLock message receiving
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task ReceivePickAndLockAsync(ServiceBusClient client)
        {
            //configure receiver for PeekLock use
            ServiceBusReceiver receiver = client.CreateReceiver(queueName, 
                new ServiceBusReceiverOptions() 
                    { ReceiveMode = ServiceBusReceiveMode.PeekLock });

            // the received message is a different type as it contains some service set properties
            ServiceBusReceivedMessage msg = await receiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(3));

            if (msg != null)
            {
                // processing message....
                var booking = JsonSerializer.Deserialize<Booking>(msg.Body);
                Console.WriteLine($"Message {msg.MessageId} received:\r\n{booking}");

                // settle the message, thereby deleting it from the service
                await receiver.CompleteMessageAsync(msg);
            }

            //release resources
            await receiver.CloseAsync();
        }

        
    }

    }
