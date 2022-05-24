using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;

namespace Server
{

    public class Program
    {
 
        /*
         * Copy connection string from output of previous script run
         */
        public static string connectionString = "<your connection string from previous script run>";



        public static async Task Main()
        {
            
            var listener = new HybridConnectionListener(connectionString, "relay");

            // callback to close the listener.
            var cancel = new CancellationTokenSource();

            // Subscribe to the status events.
            listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
            listener.Offline += (o, e) => { Console.WriteLine("Server Offline"); };
            listener.Online += (o, e) => { Console.WriteLine("Server Online"); };

            // Provide an HTTP request handler
            listener.RequestHandler = (context) =>
            {
                // Do something with context.Request.Url, HttpMethod, Headers, InputStream...
                context.Response.StatusCode = HttpStatusCode.OK;
                context.Response.StatusDescription = "OK";
                using (var sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.WriteLine("hello from the server....");
                }

                // The context MUST be closed here
                context.Response.Close();
            };

            // Opening the listener establishes the control channel to
            // the Azure Relay service. The control channel is continuously 
            // maintained, and is reestablished when connectivity is disrupted.
            await listener.OpenAsync();
            Console.WriteLine("Server listening");

            // Start a new thread that will continuously read the console.
            await Console.In.ReadLineAsync();

            // Close the listener after you exit the processing loop.
            await listener.CloseAsync();
        }

    }

}