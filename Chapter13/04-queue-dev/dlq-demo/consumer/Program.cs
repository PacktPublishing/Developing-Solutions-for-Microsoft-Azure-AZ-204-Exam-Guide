using System;
using System.Linq;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Encodings;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;
using System.Collections.Generic;
using System.Transactions;

namespace consumer
{
    class Program
    {
        /*
        /  update connectionString value with output of previous script run. 
        */
        static string connectionString = "<your connection string from previous script run>";
        
        
        static string advQueue = "advanced-queue";
        static string simplQueue = "simple-queue";

        static async Task Main(string[] args)
        {            
            // configure admin client
            ServiceBusAdministrationClient adminClint = new ServiceBusAdministrationClient(connectionString);
            // configure transaction client
            ServiceBusClient client = new ServiceBusClient(connectionString, new ServiceBusClientOptions { EnableCrossEntityTransactions = true });
            //configure non-transaction client
            ServiceBusClient nonTransactionClient = new ServiceBusClient(connectionString);

            while (true)
            {
                QueueRuntimeProperties advRuntimeProp = await adminClint.GetQueueRuntimePropertiesAsync(advQueue);
                QueueRuntimeProperties simplRuntimeProp = await adminClint.GetQueueRuntimePropertiesAsync(simplQueue);

                Console.WriteLine($"\r\n{advQueue} messages count: { advRuntimeProp.ActiveMessageCount }");
                Console.WriteLine($"{simplQueue} messages count: { simplRuntimeProp.ActiveMessageCount }");
                Console.WriteLine($"DLQ messages count: { advRuntimeProp.DeadLetterMessageCount }");

                Console.WriteLine("\r\nChose [1-4] for demonstration:");
                Console.WriteLine("\t1 - Receive and Send in transaction");
                Console.WriteLine("\t2 - Used Dead-letter queue");
                Console.WriteLine("\t3 - Receive from Dead-letter queue");
                Console.WriteLine("\t4 - Batch Peek all messages");
                Console.WriteLine("Chose [1-4]");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        await ReceiveInTransaction(client);
                        break;
                    case ConsoleKey.D2:
                        await FailReceive(nonTransactionClient);
                        break;
                    case ConsoleKey.D3:
                        await RecieveDeadLetter(nonTransactionClient);
                        break;
                    case ConsoleKey.D4:
                        await PeekAsync(nonTransactionClient);
                        break;
                }
            }
        }

        /// <summary>
        /// The function pull messages from DLQ
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task RecieveDeadLetter(ServiceBusClient client)
        {
            //create a receiver pointed to DLQ
            ServiceBusReceiver dlqReceiver = client.CreateReceiver(advQueue, new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter
            });

            //pull msg
            ServiceBusReceivedMessage dlqMessage = await dlqReceiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(3));

            if (dlqMessage != null)
            {
                // processing message....
                var booking = JsonSerializer.Deserialize<Booking>(dlqMessage.Body);
                Console.WriteLine($"DQL Message received:\r\n{booking}");
                // delete messages from the queue
                await dlqReceiver.CompleteMessageAsync(dlqMessage);
            }
            //release
            await dlqReceiver.CloseAsync();
        }

        /// <summary>
        /// The function will list all msg in both queue
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task PeekAsync(ServiceBusClient client)
        {

            foreach (var queName in new[] { simplQueue, advQueue })
            {
                //create receiver
                ServiceBusReceiver receiver = client.CreateReceiver(queName);

                //pull batch of msg
                var messages = await receiver.PeekMessagesAsync(32);

                Console.WriteLine($"\r\nMessage in queue '{queName}'");

                foreach (var msg in messages)
                {
                    var booking = JsonSerializer.Deserialize<Booking>(msg.Body);
                    Console.WriteLine($"Message (sessionId: {msg.SessionId})  peeked:\r\n{booking}");
                }

                //release resource
                await receiver.CloseAsync();
            }
        }


        /// <summary>
        /// This function will demonstrate pumping messages from simple to advanced queue
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task ReceiveInTransaction(ServiceBusClient client)
        {
            //create sender for advanced queue
            ServiceBusSender sender = client.CreateSender(advQueue);
            //create receiver for simple queue
            ServiceBusReceiver receiver = client.CreateReceiver(simplQueue);

            //pull msg from simple queue
            var firstMsg = await receiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(3));

            if (firstMsg != null)
            {
                // processing message....
                var booking = JsonSerializer.Deserialize<Booking>(firstMsg.Body);
                Console.WriteLine($"Adv Message received:\r\n{booking}");

                //start transaction
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // crate a msg for advanced queue with session support
                    ServiceBusMessage secondMsg = new ServiceBusMessage(firstMsg) { SessionId = "transaction-demo" };

                    //send msg
                    await sender.SendMessageAsync(secondMsg);

                    Console.WriteLine($"Msg submitted in adv queue.");

                    //delete msg in simple queue
                    await receiver.CompleteMessageAsync(firstMsg);

                    //commit transaction
                    ts.Complete();
                }
            }


            //release session
            await receiver.CloseAsync();
            await sender.CloseAsync();
        }


        /// <summary>
        /// The function will demonstrate how to use Dead-letter queue for poison msg
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task FailReceive(ServiceBusClient client)
        {
            //create receiver
            ServiceBusReceiver receiver = await client.AcceptNextSessionAsync(advQueue);

            //pull a message
            var msg = await receiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(3));

            // processing message....
            var booking = JsonSerializer.Deserialize<Booking>(msg.Body);
            Console.WriteLine($"Message received:\r\n{booking}");

            try
            {
                //possible nullref exception
                Console.WriteLine($"booking contains flights: {booking.AirBookings.Length} , hotels: {booking.HotelBookings.Length}");
            }
            catch (Exception ex)
            {
               Console.WriteLine("Exception during processing, move msg to DQL");

               //can not properly process the message and move it DQL
               await receiver.DeadLetterMessageAsync(msg);
            }

            //release session
            await receiver.CloseAsync();
        }


    }

}
