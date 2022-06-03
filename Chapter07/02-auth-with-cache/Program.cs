using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

// App registration variables
const string _clientId = "Put your app/client ID here";
const string _tenantId = "Put your tenant ID here";

// Cache variables
string _cacheFileLocation = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AZ-204");
const string _cacheFilename = "az204-auth.cache";

// Initiatize public client app with MSAL 
var app = PublicClientApplicationBuilder
    .Create(_clientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
    .WithRedirectUri("http://localhost")
    .Build();

// Storage for local token cache
var storageProperties =
    new StorageCreationPropertiesBuilder(
    _cacheFilename,
    _cacheFileLocation)
    .Build();

// Create cache directory
System.IO.Directory.CreateDirectory(_cacheFileLocation);

// Create CacheHelper instance
var cacheHelper = await MsalCacheHelper
    .CreateAsync(storageProperties).ConfigureAwait(false);

// Register cache with client application
cacheHelper.RegisterCache(app.UserTokenCache);

// These scopes can be added to for demonstration purposes
string[] scopes = { "user.read" };

// Call the method that tries to obtain tokens silently from the user cache
// If unable to use a token from cache, it will prompt for interactive login
var auth = await ObtainTokenAsync(app);

// Output the ID and access tokens to console
Console.WriteLine($"ID:\n{auth.IdToken}");
Console.WriteLine($"Access:\n{auth.AccessToken}");

// Method to obtain a token, trying to obtain from the local user token cache first, and then interactively if unable to from cache
async Task<AuthenticationResult> ObtainTokenAsync(IPublicClientApplication app)
{
    // Returns all the available accounts in the user token cache for the application
    IEnumerable<IAccount>? accounts = await app.GetAccountsAsync();
    // Try to get a token silently from the user token cache
    try
    {
        Console.WriteLine("Trying to get token from cache...");
        AuthenticationResult result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
        Console.WriteLine("Token acquired from cache successfully.");
        return result;
    }
    // If we need to get the token interactively, MsalUiRequiredException will be thrown
    catch(MsalUiRequiredException)
    {
        // Try to get a token interactively and set up authentication provider for Graph call
        try
        {
            Console.WriteLine("No token found in local cache. Trying to get token interactively...");
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            Console.WriteLine("Token acquired successfully.");
            return result;
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