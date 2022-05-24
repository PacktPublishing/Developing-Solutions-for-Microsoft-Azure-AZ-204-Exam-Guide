using System;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Encodings;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;

namespace publisher
{
    class Program
    {
        /*
         /  update connectionString value with output of previous script run. 
         */
        static string connectionString = "<your connection string from previous script run>";
       
        static string queueName = "simple-queue";
        static string[] citylist = new[] { "NYC", "London", "New Delhi", "Beijing", "Sydney" };


        static async Task Main()
        {
            // configure client
            ServiceBusClient client = new ServiceBusClient(connectionString);

            // create the sender
            ServiceBusSender sender = client.CreateSender(queueName);

            var i = 0;
            foreach (var city in citylist)
            {
                // serialize booking object
                var msg = JsonSerializer.Serialize(new Booking { HotelBookings = new[] { new Booking.HotelBooking() { City = city, CheckinDate = DateTime.Now, LeaveDate = DateTime.Now.AddDays(1) } } });

                // create a message that we can send.
                // UTF-8 encoding is used when providing a string.
                ServiceBusMessage message = new ServiceBusMessage(msg);

                // send the message
                await sender.SendMessageAsync(message);

                Console.WriteLine($"Message #{++i} was sent");
            }

            //release resources
            await sender.CloseAsync();

        }
    }
}
