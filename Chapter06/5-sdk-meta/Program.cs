using System;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;

namespace TheCloudShopsMetaData
{
    public class Orders
    {
       public int Id { get; set; }
       public string CustomerName { get; set; }
    }

    class Program
    {
        //update connection string from keys section on your storage account. Account was build in previous demos
        static string connectionString = "<your account connection string>";
        static string containerName = "orders";

        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            Console.WriteLine("Part 1: Creating customers");
            
            // Create Container Client 
            var container = await BuildContainer(blobServiceClient);

            //create a few orders to be stored as files.
            Orders order1 = new Orders() { Id = 1, CustomerName = "Woodrow" };
            Orders order2 = new Orders() { Id = 2, CustomerName = "ContosoLLC" };
            Orders order3 = new Orders() { Id = 3, CustomerName = "Woodrow" };

            //upload orders with meta data in the container
            await UploadCustomer(order1, container);
            await UploadCustomer(order2, container);
            await UploadCustomer(order3, container);

            Console.WriteLine("Part 2: Search for customers");

            //search for order by customer Woodrow 
            var list = await FindTheCustomer("Woodrow", blobServiceClient);

            foreach(var customer in list)
            {
                Console.WriteLine($"Found id = {customer.Id}");
            }

            Console.WriteLine("Part 3: Pull meta data");

            //read container meta data
            GetContainerMetadata(blobServiceClient);

        }


        /// <summary>
        /// Get container meta data
        /// </summary>
        /// <param name="blobServiceClient"></param>
        static void GetContainerMetadata(BlobServiceClient blobServiceClient)
        {
            //create blob client to read properties
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            //reading properties / meta data saved earlier
            var prop = containerClient.GetProperties();

            Console.WriteLine($"Container Name: {containerClient.Name}");

            var metadata = prop.Value.Metadata;

            foreach (var key in metadata.Keys)
            {
                Console.WriteLine($"\tmetadata: {key}={metadata[key]}");
            }
        }

        /// <summary>
        /// Build container with meta data
        /// </summary>
        /// <param name="blobServiceClient"></param>
        /// <returns></returns>
        static  async Task<BlobContainerClient> BuildContainer( BlobServiceClient blobServiceClient)
        {
            // Create container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Generate Meta data
            Dictionary<string, string> containerMetaData = new Dictionary<string, string>()
            {
                { "Creator" , "TheCloudShopsMetaData"  },
                { "Department", containerName  }
            };

            // Create container if it does not exits
            await containerClient.CreateIfNotExistsAsync();

            // Update container meta data
            await containerClient.SetMetadataAsync(containerMetaData);

            return containerClient;
        }

        /// <summary>
        /// Searching by indexed tags 
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        static async Task<List<Orders>> FindTheCustomer(string CustomerName, BlobServiceClient client)
        { 
            // build a search string for indexed properties
            var findCustomerQuery = @$"@container = '{containerName}' AND ""CustomerName"" = '{CustomerName}'";

            // search by query in indexed tags
            var customerResult = client.FindBlobsByTagsAsync(findCustomerQuery);

            List<Orders> customers = new List<Orders>();

            await foreach (TaggedBlobItem taggedBlobItem in customerResult)
            {
                Console.WriteLine($"BlobIndex search find BlobName={taggedBlobItem.BlobName} with tag {taggedBlobItem.Tags["CustomerName"]}");
                // pull the container and blob reference
                BlobContainerClient container = client.GetBlobContainerClient(taggedBlobItem.BlobContainerName);
                BlobClient blob = container.GetBlobClient(taggedBlobItem.BlobName);

                // download row data
                var bindata = (await blob.DownloadContentAsync()).Value.Content;
                // deserialize object 
                Orders customer = bindata.ToObjectFromJson<Orders>();

                customers.Add(customer);
            }

            return customers;
        }


        /// <summary>
        /// Create customer file with meta data and indexed prop
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        static async Task UploadCustomer(Orders customer, BlobContainerClient container)
        {
            // serialize object to store
            var content = JsonConvert.SerializeObject(customer);

            // get row data
            var binData = BinaryData.FromString(content);

            Dictionary<string, string> blobMetaData = new Dictionary<string, string>()
            {
                    { "Creator" , "TheCloudShopsMetaData" },
                    { "Department", "R&D"  },
                    { "Status", "Active"  }
            };

            Dictionary<string, string> searchTags = new Dictionary<string, string>()
            {
                 { "CustomerName", customer.CustomerName }
            };

            // setup upload options
            // overwrite blob if exist
            BlobUploadOptions opt = new BlobUploadOptions() {
                AccessTier = AccessTier.Cool,
                Metadata = blobMetaData,
                Tags = searchTags
            };

            // create a blob
            var blob = container.GetBlobClient($"customer-{customer.Id}");
            await blob.UploadAsync(binData, opt);     

        }
    }
}

