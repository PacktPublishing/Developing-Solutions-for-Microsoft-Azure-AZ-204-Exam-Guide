using System;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;

namespace TheCloudShopsAI.Data
{
    public class BlobRepo
    {
        private readonly string containerName = "thecloudshops";
        private readonly string blobConnection;

        private static BlobRepo instance;

        public static BlobRepo GetInstance
        {
            get
            {
                if (instance == null)
                    instance = new BlobRepo();

                return instance;
            }
        }

        public BlobRepo()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Environment.CurrentDirectory)
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();

            blobConnection = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("BlobConnection")) ? configuration.GetConnectionString("BlobConnection") : Environment.GetEnvironmentVariable("BlobConnection");
           
        }
        public void Initilize()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnection);

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Check if exists 
            containerClient.CreateIfNotExists();
        }

        public async Task<BlobContainerClient> GetContainer()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnection);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            return containerClient;
        }

        internal bool DeleteDescription(string filename, BlobContainerClient container)
        {
            var client = container.GetBlobClient(filename);
            return client.DeleteIfExists();
        }

        public async Task SaveDescription(string filename, string content, BlobContainerClient container)
        {
            var client = container.GetBlobClient(filename);
            client.DeleteIfExists();
            var data = new BinaryData(Encoding.UTF8.GetBytes(content));
            await client.UploadAsync(data);
        }

        public async Task<string> GetDescription(string filename,  BlobContainerClient container)
        {
            var client = container.GetBlobClient(filename);
            var data = await client.DownloadContentAsync();
            return Encoding.UTF8.GetString(data.Value.Content);      
        }
    }


}

