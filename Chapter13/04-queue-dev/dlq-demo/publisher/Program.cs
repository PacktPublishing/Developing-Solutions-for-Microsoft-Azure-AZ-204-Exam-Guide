using System;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;

namespace publisher
{
    
    class Program
    {
        /*
        /  update connectionString value with output of previous script run. 
        */
        static string connectionString = "<your connection string from previous script run>";
       
        
        static string firstQueueName = "simple-queue";
        static string secondQueueName =  "advanced-queue";
        static string[] citylist = new[] { "Miami", "NYC", "London", "Paris", "Caracas" };


        static async Task Main()
        {
            // configure client
            ServiceBusClient client = new ServiceBusClient(connectionString, 
                new ServiceBusClientOptions { EnableCrossEntityTransactions = true });
                        
            ServiceBusSender firstSender = client.CreateSender(firstQueueName);
            ServiceBusSender secondSender = client.CreateSender(secondQueueName);


            var i = 0;
            foreach (var city in citylist)
            {
               //submitting messages in transactions
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // serialize booking object
                    string msg = JsonSerializer.Serialize(new Booking { HotelBookings = new[] { new Booking.HotelBooking() { City = city, CheckinDate = DateTime.Now, LeaveDate = DateTime.Now.AddDays(1) } } });

                    
                    ServiceBusMessage messageOene = new ServiceBusMessage(msg);
                    //send in first queue
                    await firstSender.SendMessageAsync(messageOene);

                    ServiceBusMessage messageTwo = new ServiceBusMessage(msg)
                    {
                        SessionId = "transaction-demo"
                    };

                    //send in second queue
                    await secondSender.SendMessageAsync(messageTwo);
                    
                    //commit
                    ts.Complete();
                }

                Console.WriteLine($"Message #{++i} was sent in both queue");
            }

            //release recourses
            await firstSender.CloseAsync();
            await secondSender.CloseAsync();
        }

    }
    
}
