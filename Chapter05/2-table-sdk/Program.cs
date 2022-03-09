using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Collections;
using System.Linq;

namespace TheCloudShopsTableDemo
{

    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "";

            // Create a new table.
            string tableName = "customerscode";
            var clinet = new TableClient(connectionString, tableName);
            clinet.CreateIfNotExists();            

            CreateOrUpsertCustomer(clinet, "WingTipToys", "ReSellers", true);
            CreateOrUpsertCustomer(clinet, "ADatum", "ReSellers", true);
            CreateOrUpsertCustomer(clinet, "FabricanFuber", "Sellers", false);

            QueryCustomers(clinet, "ReSellers");
            QueryActiveCustomers(clinet);

            DeleteItemAndTable(clinet, "ADatum", "ReSellers");
        }

        private static void DeleteItemAndTable(TableClient clinet, string CName, string CType)
        {
            clinet.DeleteEntity(CType, CName); //delete entity

            Console.WriteLine("Press key for delete table");
            Console.ReadLine();

            clinet.Delete(); //delete table
            Console.WriteLine("Table deleted");

        }

        private static void QueryActiveCustomers(TableClient tableClient)
        {
            Console.WriteLine("Query filter: IsActive eq false");

            //query by field value
            Pageable <TableEntity> results = tableClient.Query<TableEntity>(filter: $"IsActive eq false");

            foreach (TableEntity entity in results)
            {
                Console.WriteLine($"{entity.GetString("RowKey")}: {entity.GetBoolean("IsActive")}");
            }

            Console.WriteLine($"The query returned {results.Count()} entities.");
        }

        private static void QueryCustomers(TableClient tableClient, string partitionKey)
        {
            Console.WriteLine($"Query filter: PartitionKey eq '{partitionKey}'");

            //query by keys
            Pageable<TableEntity> results = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{partitionKey}'");

            foreach (TableEntity entity in results)
            {
                Console.WriteLine($"{entity.GetString("RowKey")}: {entity.GetBoolean("IsActive")}");
            }

            Console.WriteLine($"The query returned {results.Count()} entities.");
        }

        private static void CreateOrUpsertCustomer(TableClient tableClient, string CName, string CType, bool IsActive)
        {
            var entity = new TableEntity(CType, CName)
            {
                    { "IsActive", IsActive },
               };

            try
            {
                //select entity if exists
                var existed = tableClient.GetEntity<TableEntity>(CType, CName);

                //entity exists so we replace it with new
                tableClient.UpsertEntity(entity, TableUpdateMode.Replace);
                Console.WriteLine("Entity replaced");
            }
            catch(RequestFailedException ex)
            {
                //entity not exist so we create new
                tableClient.AddEntity(entity);
                Console.WriteLine("Entity created");
            }

        }
    }
}
