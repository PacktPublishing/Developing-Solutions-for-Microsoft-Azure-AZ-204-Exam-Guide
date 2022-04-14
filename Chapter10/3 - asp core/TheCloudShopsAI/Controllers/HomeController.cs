using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheCloudShopsAI.Data;
using TheCloudShopsAI.Models;

namespace TheCloudShopsAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ShopContext _context;
        private TelemetryClient _telemetry;

        public HomeController(ILogger<HomeController> logger, ShopContext context, TelemetryClient client)
        {
            _context = context;
            _telemetry = client;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Privacy));
        }

        public IActionResult Privacy()
        {
            try
            {
                var order = _context.Orders.Find(1234);
                return View(order.Client);
            }catch(Exception ex)
            {
                _telemetry.TrackException(ex);
                throw; //trow the exception to demonstrate Error page.
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
