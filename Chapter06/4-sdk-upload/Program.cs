using System;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace TheCloudShopsBlobs
{
    class Program
    {
        static string connectionString = "<your connection string>";

        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            //Create a unique name for the container
            string containerName = "thecloudshops";

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Check if exists 
            await containerClient.CreateIfNotExistsAsync();

            //Upload files from disk on the local folder to cloud
            await Upload("logo.png", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png"), containerClient);

            //Download file from the blob to the disk
            await Download("logo.png", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dwn-logo.png"), containerClient);


            Console.WriteLine("Press key to delete file on blob?");
            Console.ReadLine();

            //delete blob
            Delete("logo.png", containerClient);
        }

        static void Delete(string BlobName, BlobContainerClient container)
        {
            BlobClient blob = container.GetBlobClient(BlobName);
            blob.DeleteIfExists();
            Console.WriteLine("File deleted");
        }

        static async Task Upload(string BlobName, string filePath, BlobContainerClient container)
        { 
            using (var file = File.OpenRead(filePath))
            {
                var client = container.GetBlobClient(BlobName);
                await client.UploadAsync(file, true)
                    .ContinueWith((result)=>Console.WriteLine($"File uploaded with status {result.Status}"));
            }
        }

        static async Task Download(string BlobName, string filePath, BlobContainerClient container)
        {
            using (var file = File.OpenWrite(filePath))
            {
                BlobClient blob = container.GetBlobClient(BlobName);
                await blob.DownloadToAsync(file)
                    .ContinueWith((result) => Console.WriteLine($"File downloaded with status {result.Status}"));
            }
        }
    }
}

