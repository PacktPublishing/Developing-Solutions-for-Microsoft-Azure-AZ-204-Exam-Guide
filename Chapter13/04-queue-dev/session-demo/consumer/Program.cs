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
                
        static string queueName = "advanced-queue";

        static async Task Main()
        {            
            // configure admin client
            ServiceBusAdministrationClient adminClint = new ServiceBusAdministrationClient(connectionString);
            // configure client
            ServiceBusClient client = new ServiceBusClient(connectionString);

            while (true)
            {
                QueueRuntimeProperties runtimeProp = await adminClint.GetQueueRuntimePropertiesAsync(queueName);

                Console.WriteLine($"\r\nCurrent messages count: { runtimeProp.ActiveMessageCount }");
                Console.WriteLine("\r\nChose [1-2] for demonstration:");
                Console.WriteLine("\t1 - Receive per session ");
                Console.WriteLine("\t2 - Batch Peek all messages");
                Console.WriteLine("Chose [1-2]");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        await ReceivePerSession(client);
                        break;
                    case ConsoleKey.D2:
                        await PeekAsync(client);
                        break;
                }
            }
        }

        /// <summary>
        /// List all messages and keep them in queue
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task PeekAsync(ServiceBusClient client)
        {
            //create a receiver as for non-session receive
            ServiceBusReceiver receiver = client.CreateReceiver(queueName);
            //pick a batch of messages
            var messages = await receiver.PeekMessagesAsync(32);

            foreach (var msg in messages)
            {
                var booking = JsonSerializer.Deserialize<Booking>(msg.Body);
                Console.WriteLine($"Message (sessionId: {msg.SessionId})  peeked:\r\n{booking}");
            }

            //release the receiver
            await receiver.CloseAsync();
        }


        /// <summary>
        /// The receiving messages per session. Next available session will be selected 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task ReceivePerSession(ServiceBusClient client)
        {
            //create a receiver for next session (sessions will be enumerated) 
            // you also can provide exact session name.
            ServiceBusReceiver receiver = await client.AcceptNextSessionAsync(queueName);

            var messages = await receiver.ReceiveMessagesAsync(maxMessages: 100, maxWaitTime: TimeSpan.FromSeconds(3));

            foreach (var msg in messages)
            {

                // processing message....
                var booking = JsonSerializer.Deserialize<Booking>(msg.Body);
                Console.WriteLine($"Message received:\r\n{booking}");
                // delete messages from the queue
                await receiver.CompleteMessageAsync(msg);
            }

            //release session
            await receiver.CloseAsync();
        }

    }

    }
