using Microsoft.Identity.Client;

const string _clientId = "Put your app/client ID here";
const string _tenantId = "Put your tenant ID here";

var app = PublicClientApplicationBuilder
    .Create(_clientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
    .WithRedirectUri("http://localhost")
    .Build();

string[] scopes = { "User.Read", "Calendars.Read" };

AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

Console.WriteLine($"ID:\n{result.IdToken}");
Console.WriteLine($"Access:\n{result.AccessToken}");
