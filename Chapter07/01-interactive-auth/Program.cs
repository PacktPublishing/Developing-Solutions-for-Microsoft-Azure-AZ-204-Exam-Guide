// MSAL
using Microsoft.Identity.Client;

// App registration variables
const string _clientId = "Put your app/client ID here";
const string _tenantId = "Put your tenant ID here";

// Initiatize public client app with MSAL
var app = PublicClientApplicationBuilder
    .Create(_clientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
    .WithRedirectUri("http://localhost")
    .Build();

// These scopes can be added to for demonstration purposes
string[] scopes = { "User.Read", "Calendars.Read" };

// Acquire a token interactively
AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

// Output the ID token and access token to console
Console.WriteLine($"ID:\n{result.IdToken}");
Console.WriteLine($"Access:\n{result.AccessToken}");
