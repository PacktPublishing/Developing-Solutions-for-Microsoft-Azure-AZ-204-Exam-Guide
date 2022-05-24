using System;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.Relay;

namespace client
{
    class Program
    {
        /*
         * Copy connection string from output of previous script run
         */
        private const string connectionString = "<your connection string from previous script run>";


        private static async Task Main()
        {
            //create a connection to generate the token
            var clientHC = new HybridConnectionClient(connectionString, "relay");

            //build https URL
            var uri = new Uri(string.Format("https://{0}/{1}", clientHC.Address.Host, "relay"));
            var token = (await clientHC.TokenProvider.GetTokenAsync(uri.AbsoluteUri, TimeSpan.FromHours(1))).TokenString;
            //create simple HTTP client
            var client = new HttpClient();

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
                
            };

            //use token
            request.Headers.Add("ServiceBusAuthorization", token);
            //get msg from server
            var response = await client.SendAsync(request);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

       
    }
}