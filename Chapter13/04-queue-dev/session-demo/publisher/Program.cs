using System;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Encodings;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;
using System.Linq;

namespace publisher
{
    class Program
    {
        /*
        /  update connectionString value with output of previous script run. 
        */
        static string connectionString = "<your connection string from previous script run>";
        
        static string queueName = "advanced-queue";
        static string[] airportlist = new[] { "MIA", "HNL", "LHR" };
        static string[] citylist = new[] { "Miami", "NYC" };
 
        static async Task Main()
        {
            // configure client
            ServiceBusClient client = new ServiceBusClient(connectionString);

            // create the sender
            ServiceBusSender sender = client.CreateSender(queueName);

            Console.WriteLine($"Use first session");

            //sending messages without session 
            var i = 0;
            foreach (var city in citylist)
            {
                // serialize booking object
                var msg = JsonSerializer.Serialize(new Booking { HotelBookings = new[] { new Booking.HotelBooking() { City = city, CheckinDate = DateTime.Now, LeaveDate = DateTime.Now.AddDays(1) } } });

                // create a message that we can send.
                // UTF-8 encoding is used when providing a string.
                ServiceBusMessage message = new ServiceBusMessage(msg)
                {
                    SessionId = "hotelbooking"
                };

                // send the message
                await sender.SendMessageAsync(message);

                Console.WriteLine($"\tMessage #{++i} was sent");
            }

            Console.WriteLine($"Use another session");

            //sending messages with session indicator
            foreach (var airport in airportlist)
            {
                // serialize booking object
                var msg = JsonSerializer.Serialize(new Booking { AirBookings = new[] { new Booking.AirBooking() { To = airport, From = "JFK", FlighDate = DateTime.Now } } });

                // create a message that we can send.
                // UTF-8 encoding is used when providing a string.
                ServiceBusMessage message = new ServiceBusMessage(msg)
                {
                    SessionId = "airbooking"
                };

                // send the message
                await sender.SendMessageAsync(message);

                Console.WriteLine($"\tMessage #{++i} was sent");
            }

            //release the resource
            await sender.CloseAsync();

        }
    }
}
