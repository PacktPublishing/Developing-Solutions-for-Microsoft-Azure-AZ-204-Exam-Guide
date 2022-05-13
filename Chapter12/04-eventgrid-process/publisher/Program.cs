using Azure;
using Azure.Messaging.EventGrid;
using System;
using System.Threading.Tasks;

namespace publisher
{
    class Program
    {
        static string endpoint = "<your custom topic endpoint>";
        static string key = "<your custom topic access key>";

        static void Main(string[] args)
        {
            Send().Wait();

        }

        static async Task Send()
        {
                EventGridPublisherClient client = new EventGridPublisherClient(
                new Uri(endpoint),
                new AzureKeyCredential(key));

                        // Add EventGridEvents to a list to publish to the topic
                        EventGridEvent egEvent =
                            new EventGridEvent(
                                "ExampleEventSubject",
                                "Example.EventType",
                                "1.0",
                                "This is the event data");

                        // Send the event
                        await client.SendEventAsync(egEvent);
        }
    }
}
