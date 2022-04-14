using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheCloudShopsAI.Models;


namespace TheCloudShopsAI.Data
{

    public static class DbInitializer
    {
        public static void Initialize(ShopContext context)
        {
            context.Database.EnsureCreated();

            // Look for any clients.
            if (!context.Clients.Any())
            {


                var clients = new Client[]
                {
                    new Client{ClientID =1, Name = "Alex"},
                    new Client{ClientID =2, Name = "John"},
                    new Client{ClientID =3, Name = "Ted"},
                    new Client{ClientID =4, Name = "Peter"},
                    new Client{ClientID =5, Name = "Natasha"},
                    new Client{ClientID =6, Name = "Julia"},
                    new Client{ClientID =7, Name = "Nino"},
                    new Client{ClientID =8, Name = "Laura"},
                    new Client{ClientID =9, Name = "Mat"}
                };
                foreach (Client c in clients)
                {
                    context.Clients.Add(c);
                }
                context.SaveChanges();
            }

            // Look for any orders.
            if (!context.Orders.Any())
            {

                var orders = new Order[]
                {
                        new Order{  ClientID = 1,   Size=Size.XL, ProductName = "Red T-Short"},
                        new Order{  ClientID = 2,   Size=Size.XL, ProductName = "Blue T-Short"},
                        new Order{  ClientID = 3,   Size=Size.M, ProductName = "Yellow T-Short"},
                        new Order{  ClientID = 4,   Size=Size.S, ProductName = "Orange T-Short"},
                        new Order{  ClientID = 5,   Size=Size.L, ProductName = "Black Short"},
                        new Order{  ClientID = 6,   Size=Size.M, ProductName = "Gray Short"},
                        new Order{  ClientID = 7,   Size=Size.M, ProductName = "Blue Short"},
                        new Order{  ClientID = 8,   Size=Size.S, ProductName = "White Hat"},
                        new Order{  ClientID = 9,   Size=Size.L, ProductName = "Green Hat"},
                        new Order{  ClientID = 1,   Size=Size.L, ProductName = "Blue Hat"},
                        new Order{  ClientID = 2,   Size=Size.S, ProductName = "Yellow Hat"},
                };


                var blob = BlobRepo.GetInstance;
                blob.Initilize();

                var container = blob.GetContainer().Result;

                foreach (Order o in orders)
                {
                    blob.SaveDescription(o.ProductName, $"{o.ProductName} in the perfect condition of size {o.Size}", container).Wait();
                    context.Orders.Add(o);
                }
                context.SaveChanges();

            }


        }
    }
}