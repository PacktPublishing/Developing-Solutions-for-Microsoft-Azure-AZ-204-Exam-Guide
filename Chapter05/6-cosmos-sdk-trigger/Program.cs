using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace TheCloudShopsTriggerTest
{
    public class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "AZ204Demo";
        private string containerId = "TheCloudShops";

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.GetStartedDemoAsync();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            //create db and containers
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();

            try
            {
                //run seed method to load documents
                await this.AddItemsToContainerWithTriggerCheckAsync();
            }
            catch (CosmosException de)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de.ResponseBody);
                Console.ForegroundColor = ConsoleColor.White;
            }
           
        }

        /// <summary>
        /// Create the container if it does not exist. 
        /// Specify "/OrderAddress/City" as the partition key since we're storing different types of documents.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = (Container)await this.database.CreateContainerIfNotExistsAsync(containerId, "/OrderAddress/City");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = (Database) await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }


        private async Task CreateDocumentsIfNotExists(Order order) 
        {
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Order> readResponse = await this.container.ReadItemAsync<Order>(order.id, new PartitionKey(order.OrderAddress.City));
                Console.WriteLine("Item in database with id: {0} already exists\n", readResponse.Resource.id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container.
                ItemResponse<Order> createResponse = await this.container.CreateItemAsync<Order>(order, new PartitionKey(order.OrderAddress.City),
                    new ItemRequestOptions(){PreTriggers= new List<string> { "validateOrder" } });

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", createResponse.Resource.id, createResponse.RequestCharge);
            }

        }
        /// <summary>
        /// Add Family items to the container
        /// </summary>
        private async Task AddItemsToContainerWithTriggerCheckAsync()
        {
            // Create a objects
            Customer customer4 = new Customer() { Name = "TriggerTestINC" , IsActive = true};
            Product product5 = new Product() { ProductName= "Covid" };


            Order orderWithCustomer = new Order()
            {
                id = "o10",
                OrderNumber = "NL-24",
                OrderCustiomer = customer4,
                OrderAddress = new Address { State = "WA", County = "King", City = "Redmond" },
                OrderItems = new[] {
                    new OrderItem() { ProductItem = product5, Count = 1 }
                }
            };

            Order orderNoCustomer = new Order()
            {
                id = "o11",
                OrderNumber = "NL-25",
                OrderCustiomer = null,  // the customer is not defined.
                OrderAddress = new Address { State = "WA", County = "King", City = "Seattle" },
                OrderItems = new[] {
                    new OrderItem() { ProductItem = product5, Count = 1 }
                }
            };


            //create orders
            await CreateDocumentsIfNotExists(orderWithCustomer);
            await CreateDocumentsIfNotExists(orderNoCustomer);
        }
    }
}
