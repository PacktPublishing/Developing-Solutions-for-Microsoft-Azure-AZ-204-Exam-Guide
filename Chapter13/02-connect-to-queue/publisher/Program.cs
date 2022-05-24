using System;
using Azure.Storage.Queues; 
using System.Text.Json;

namespace publisher
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
            Console.WriteLine("Your publisher just started!");

            var client = CreateQueueClient(); //build client

            // sending 30 messages 
            for( var i=0; i< 30; i++ )
            {
               string msg = JsonSerializer.Serialize( new TheMessage(){ MsgID = $"{i}", Info = $"Simple messaging #{i}"});
               InsertMessage(client, msg);
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


        public static void InsertMessage(QueueClient queueClient, string message)
        {
            //sending messages from 
            queueClient.SendMessage(message);

            Console.WriteLine($"Message inserted");
        }      
    }
}
