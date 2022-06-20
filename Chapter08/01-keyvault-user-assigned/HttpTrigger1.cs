using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Company.Function
{
    public static class HttpTrigger1
    {
        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userAssignedClientId = "Your user-assigned managed identity's client ID";
            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });

            var client = new SecretClient(new Uri("https://<vault name>.vault.azure.net/"), credential);

            KeyVaultSecret secret = await client.GetSecretAsync("<secret name>");

            string responseMessage = $"Secret value: { secret.Value }";

            return new OkObjectResult(responseMessage);
        }
    }
}
