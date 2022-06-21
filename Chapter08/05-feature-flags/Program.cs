using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var connectionString = builder.Configuration.GetConnectionString("AppConfig");

builder.Host.ConfigureAppConfiguration((hostingContext, builder) =>
{
    builder.AddAzureAppConfiguration(options =>
    {
        options.Connect(connectionString)
               .UseFeatureFlags(option =>
               {
                option.Select("demofeature", hostingContext.HostingEnvironment.EnvironmentName);
               })
               .ConfigureRefresh(refreshOptions =>
               {
                   refreshOptions.Register("demofeature");
               })
               .Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName);
    });
});

builder.Services.AddAzureAppConfiguration()
                .AddFeatureManagement();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Use the App Configuration middleware for dynamic refresh
app.UseAzureAppConfiguration();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
