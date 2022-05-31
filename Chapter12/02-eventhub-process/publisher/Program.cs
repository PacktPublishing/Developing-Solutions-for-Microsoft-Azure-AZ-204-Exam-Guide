using System;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using System.Text;

namespace publisher
{
 class Program
    {
        private static EventHubClient client;
        private const string EventHubConnectionString = "<you event hub connection string from previous script run>";

        private static async Task Main(string[] args)
        {
            // Creates an EventHubsConnectionStringBuilder object from a the connection string, and sets the EntityPath.
            // Typically the connection string should have the Entity Path in it, but for the sake of this simple scenario
            // we are using the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString);

            client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendEventsToEventHubAsync(10);

            await client.CloseAsync();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        // Creates an Event Hub client and sends 10 messages to the event hub.
        private static async Task SendEventsToEventHubAsync(int numMsgToSend)
        {
            for (var i = 0; i < numMsgToSend; i++)
            {
                try
                {
                    var message = $"Event #{i}";
                    Console.WriteLine($"Sending event: {message}");
                    await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMsgToSend} events sent.");
        }
    }
}
