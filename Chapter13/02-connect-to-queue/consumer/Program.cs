using System;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;

namespace consumer
{

    //custom class to serialize in the message
    public class TheMessage
    {
        public string MsgID { get; set; }
        public string Info { get; set; }
    }

    class Program
    {
         /*
         * Copy connection string from output of previous script run
         */
        static string connectionString = "<your connection string from previous script run>";
        
        
        static string queueName = "demo";

        static void Main(string[] args)
        {
            var client = CreateQueueClient();

            while (true)
            {
                QueueProperties prop = client.GetProperties();

                Console.WriteLine($"Current messages count: {prop.ApproximateMessagesCount}");
                Console.WriteLine("\r\nChose [1-3] for demonstration:");
                Console.WriteLine("\t1 - Receive and delete ");
                Console.WriteLine("\t2 - Receive, update then delete");
                Console.WriteLine("\t3 - Batch Peek messages");
                Console.WriteLine("Chose [1-3]");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        ReceiveAndDelete(client);
                        break;

                    case ConsoleKey.D2:
                        ReceiveHideThenDelete(client);
                        break;

                    case ConsoleKey.D3:
                        Peek(client);
                        break;
                }
                
            }

           
        }

        public static QueueClient CreateQueueClient()
        {
            try
            {
                
                QueueClient queueClient = new QueueClient(connectionString, queueName);

                queueClient.CreateIfNotExists();    // Create the queue

                if (queueClient.Exists())
                    Console.WriteLine($"The queue created: '{queueClient.Name}'");

                return queueClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n\n");
                throw;
            }
        }

        public static void Peek(QueueClient queueClient)
        {
            Console.WriteLine($"\tMax number of peekable msg: {queueClient.MaxPeekableMessages}");

            // Peek at the messages
            PeekedMessage[] peekedMessages = queueClient.PeekMessages(maxMessages: queueClient.MaxPeekableMessages);

            foreach(var m in peekedMessages)
            {
                var msg = JsonSerializer.Deserialize<TheMessage>(m.Body);

                Console.WriteLine($"\tRead message #{msg.MsgID}");

            }
        }


        public static void ReceiveHideThenDelete(QueueClient queueClient)
        {
            for (int i = 0; i < 3; i++)
            {
                // Get the next message
                // Default visibility 30 sec
                QueueMessage retrievedMessage = queueClient.ReceiveMessage(); 
                if (retrievedMessage != null)
                {
                    var msg = JsonSerializer.Deserialize<TheMessage>(retrievedMessage.Body);

                    // Process the message
                    Console.WriteLine($"\tUpdate message #{msg.MsgID}");


                    //realize that processing take a longer time and update visibility
                    UpdateReceipt rc = queueClient.UpdateMessage(
                        retrievedMessage.MessageId,
                        retrievedMessage.PopReceipt,
                        visibilityTimeout: TimeSpan.FromMinutes(5));


                    // Delete the message with receipt from update
                    queueClient.DeleteMessage(retrievedMessage.MessageId, rc.PopReceipt);

                    Console.WriteLine($"\tDelete message #{msg.MsgID}");
                }
                else
                {
                    Console.WriteLine($"No more messages found.");
                }
            }
        }

        public static void ReceiveAndDelete(QueueClient queueClient)
        {
            for (int i = 0; i < 3; i++)
            {
                // Get the next message
                QueueMessage retrievedMessage = queueClient.ReceiveMessage();

                if (retrievedMessage != null)
                {
                    var msg = JsonSerializer.Deserialize<TheMessage>(retrievedMessage.Body);

                    // Process the message
                    Console.WriteLine($"\tDequeued message #{msg.MsgID}");

                    // Delete the message with receipt
                    queueClient.DeleteMessage(retrievedMessage.MessageId, retrievedMessage.PopReceipt);

                    Console.WriteLine($"\tDelete message #{msg.MsgID}");
                }
                else
                {
                    Console.WriteLine($"No more messages found.");
                }
            }
        }        
    }
}
