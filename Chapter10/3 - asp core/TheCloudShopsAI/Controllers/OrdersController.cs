using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheCloudShopsAI.Data;
using TheCloudShopsAI.Models;

namespace TheCloudShopsAI.Controllers
{
    public class OrdersController : Controller
    {
        private TelemetryClient telemetry;
     
        private readonly ShopContext _context;

        public OrdersController(ShopContext context, TelemetryClient client)
        {
            telemetry = client;
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var list = await _context.Orders.ToListAsync();
            return View(list);
        }

        // GET: Orders/Details/5
        public ActionResult DetailsAsync(int id)
        {
            var order = _context.Orders.Find(id);
            order.Client = _context.Clients.Find(order.ClientID);
            var blob = BlobRepo.GetInstance;
            order.Description = blob.GetDescription(order.ProductName, blob.GetContainer().Result).Result;

            telemetry.TrackTrace($"Details loaded for Order #{id}");

            return View(order);
        }

        // GET: Orders/Edit
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.Clients = _context.Clients.ToList();
            var order = await _context.Orders.FindAsync(id);
            var blob = BlobRepo.GetInstance;
            order.Description = blob.GetDescription(order.ProductName, blob.GetContainer().Result).Result;
            return View(order);
        }

        // GET: Orders/Delete
        public async Task<ActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            var blob = BlobRepo.GetInstance;
            blob.DeleteDescription(order.ProductName, blob.GetContainer().Result);

            telemetry.TrackTrace($"Order #{id} Deleted");
            return RedirectToAction(nameof(Index));
        }


        // POST: Orders/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            try
            {
                //save
                var id = int.Parse(collection["ID"].First());
                Order order = await _context.Orders.FindAsync(id);

                order.Size = (Size)Enum.Parse(typeof(Size), collection["Size"].First());
                order.ProductName = collection["ProductName"].First();
                order.Description = collection["Description"].First();
                order.ClientID = int.Parse(collection["ClientID"].First());

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                var blob = BlobRepo.GetInstance;
                await blob.SaveDescription(order.ProductName, order.Description, blob.GetContainer().Result);

                telemetry.TrackTrace($"Order #{id} Update");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.Clients = _context.Clients.ToList();
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var order = new Order()
                {
                    ProductName = collection["ProductName"].First(),
                    Size = (Size)Enum.Parse(typeof(Size), collection["Size"].First()),
                    ClientID = int.Parse(collection["ClientID"].First()),
                    Description = collection["Description"].First(),
                };

                order.Client = _context.Clients.Find(order.ClientID);

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var blob = BlobRepo.GetInstance;
                await blob.SaveDescription(order.ProductName, order.Description, blob.GetContainer().Result);

                telemetry.TrackTrace("New Order created");

                telemetry.TrackEvent("New Order", new Dictionary<string, string>() {
                    { "Product" , order.ProductName },
                    {  "Size" , order.Size.ToString() },
                    {  "Client" , order.Client.Name },
                });
                    
                telemetry.TrackMetric( $"Client {order.Client.Name}", 1 );

                telemetry.TrackMetric("Orders", 1);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


    }
}
