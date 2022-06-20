using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var connectionString = builder.Configuration.GetConnectionString("AppConfig");

builder.Host.ConfigureAppConfiguration((hostingContext, builder) =>
{
    builder.AddAzureAppConfiguration(options =>
    {
        options.Connect(connectionString)
               .Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName);
    });
});

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

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
