using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;

namespace TheCloudShopWebState
{
    public class StartupRedis
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostContext;

        public StartupRedis(IConfiguration config, IWebHostEnvironment hostContext)
        {
            _config = config;
            _hostContext = hostContext;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(60);  //how long the redis persist the session 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //configure Redis instance in Azure
            services.AddStackExchangeRedisCache(options =>
            {
                if (String.IsNullOrEmpty(_config["AzureRedis"]))
                    throw new Exception("Redis instance is not configured in appsettings.json ");

                options.Configuration = _config["AzureRedis"];  //init session from connection string
                options.InstanceName = "DemoInstanse";
            });

            services.AddRazorPages();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IHostApplicationLifetime lifetime, IDistributedCache cache)
        {
            lifetime.ApplicationStarted.Register(() =>
            {
                var ServerStartTime = DateTime.UtcNow.ToString();
                byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(ServerStartTime);

                //20 seconds to persist value in cache
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));

                //persist start time in cache
                cache.Set("cachedTimeUTC", encodedCurrentTimeUTC, options);
            });


            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
