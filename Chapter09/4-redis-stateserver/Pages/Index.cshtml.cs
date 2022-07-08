using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TheCloudShopWebState
{
    public class IndexModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public IndexModel(IDistributedCache cache)
        {
            _cache = cache;
            TheMSGKey = "TheMSG";
            TheMSGsetTimeKey = "TheMSGsetTime";
            TheMSGsetSessionKey = "TheMSGsetSession";
        }



        public string TheMSGKey { get; set; }
        public string TheMSGsetTimeKey { get; set; }
        public string TheMSGsetSessionKey { get; set; }

        public string CurrentSessionID { get; set; }

        /// <summary>
        /// setup values to read them back later
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            HttpContext.Session.Set("dummy", Encoding.UTF8.GetBytes("value")); // to keep the session id 
            CurrentSessionID = HttpContext.Session.Id; 
        }

        public async Task<IActionResult> OnPostSaveSessionValue()
        {
            var value = HttpContext.Request.Form["msg"].ToString();
            HttpContext.Session.SetString(TheMSGKey, value);
            HttpContext.Session.SetString(TheMSGsetTimeKey, DateTime.Now.ToString());
            HttpContext.Session.SetString(TheMSGsetSessionKey, HttpContext.Session.Id);

            return RedirectToPage();
        }  
    }

}
