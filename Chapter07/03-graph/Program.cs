using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Microsoft.Graph;

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
string[] scopes = { "User.Read" };

// Configure authentication provider for making MS Graph calls
DelegateAuthenticationProvider authProvider = new DelegateAuthenticationProvider(async (request) => {
    try
    {
        // Construct the Authorization header for MS Graph calls
        try
        {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await ObtainTokenAsync(app));
        }
        // Catch exceptions
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    // Catch exceptions
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
});

// Create a new GraphServiceClient using the authentication provider created
var graphClient = new GraphServiceClient(authProvider);

// Use the Graph SDK to call https://graph.microsoft.com/me for returning the logged-in user's profile
var user = await graphClient.Me.Request().GetAsync();

// Customized greeting for the logged-in user
Console.WriteLine($"Hello, {user.GivenName}! Your name was obtained from MS Graph.\n"); 

// Method to obtain a token, trying to obtain from the local user token cache first, and then interactively if unable to from cache
async Task<string> ObtainTokenAsync(IPublicClientApplication app)
{
    // Returns all the available accounts in the user token cache for the application
    IEnumerable<IAccount>? accounts = await app.GetAccountsAsync();
    // Try to get a token silently from the user token cache
    try
    {
        Console.WriteLine("Trying to get token from cache...");
        AuthenticationResult result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
        Console.WriteLine("Token acquired from cache successfully.\n");
        return result.AccessToken;
    }
    // If we need to get the token interactively, MsalUiRequiredException will be thrown
    catch(MsalUiRequiredException)
    {
        // Try to get a token interactively and set up authentication provider for Graph call
        try
        {
            Console.WriteLine("No token found in local cache. Trying to get token interactively...");
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            Console.WriteLine("Token acquired successfully.\n");
            return result.AccessToken;
        }
        // Catch MSAL exceptions
        catch(MsalException ex)
        {
            Console.WriteLine($"Failed to get token interactively: {Environment.NewLine}{ex.Message}");
            throw;
        }
    }
    // Catch MSAL exceptions
    catch(MsalException ex)
    {
        Console.WriteLine($"Failed to get token silently: {Environment.NewLine}{ex.Message}");
        throw;
    }
}
