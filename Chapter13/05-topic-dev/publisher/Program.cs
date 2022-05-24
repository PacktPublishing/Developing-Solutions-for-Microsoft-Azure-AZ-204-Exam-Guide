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
        
        static string topicName = "booking";
        static string[] airportlist = new[] { "MIA", "JFK", "LHR", "FLL", "DLG" };
        static string[] citylist = new[] { "Miami", "NYC", "London", "Fort Lauderdale", "Paris" };

        static Random rnd = new Random(DateTime.Now.Millisecond);
 
        static async Task Main()
        {


            // configure client
            ServiceBusClient client = new ServiceBusClient(connectionString);

            // create the sender
            ServiceBusSender sender = client.CreateSender(topicName);
            var n = 0;
            while (true)
            {

                //sending messages without session 
                for (var i = 0; i < 10; i++)
                {
                    // serialize booking object
                    var msg = JsonSerializer.Serialize(new Booking
                    {
                        HotelBookings = new[] { new Booking.HotelBooking() { City = citylist[rnd.Next(0, 5)], CheckinDate = DateTime.Now, LeaveDate = DateTime.Now.AddDays(1) } },
                        AirBookings = new[] { new Booking.AirBooking() { From = airportlist[rnd.Next(0, 5)], FlighDate = DateTime.Now } }
                    });

                    // create a message that we can send.
                    ServiceBusMessage message = new ServiceBusMessage(msg);

                    // send the message
                    await sender.SendMessageAsync(message);

                    Console.WriteLine($"Message #{++n} was sent");
                }

                Console.WriteLine($"press any key to send more.....");
                Console.ReadKey();
            }
            
            //release the resource
            await sender.CloseAsync();

        }
    }
}
