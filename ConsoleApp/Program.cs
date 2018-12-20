using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.CosmosDB.Fluent;
using Microsoft.Azure.Management.CosmosDB.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Microsoft.Azure.Management.Samples.Common;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBWithEventualConsistency
{

    public class Program
    {
        public static async Task WalkCosmosAccounts(IAzure azure)
        {

            try
            {
                foreach (var cosmosDBAccount in azure.CosmosDBAccounts.List())
                {
                    var databaseAccountListKeysResult = cosmosDBAccount.ListKeys();
                    string masterKey = databaseAccountListKeysResult.PrimaryMasterKey;
                    string endPoint = cosmosDBAccount.DocumentEndpoint;

                    var client = new DocumentClient(new Uri(endPoint), masterKey, ConnectionPolicy.Default);
                    var databases = await client.ReadDatabaseFeedAsync();

                    Console.WriteLine("Reading all databases resources for - " + cosmosDBAccount.Name);
                    foreach (var db in databases)
                    {
                        List<DocumentCollection> collections = client.CreateDocumentCollectionQuery((String)db.SelfLink).ToList();
                        Console.WriteLine("\tDatabase Name - " + db.Id);
                        foreach (var col in collections)
                        {
                            Console.WriteLine("\t\tCollection Name - " + col.Id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.Log(e.Message);
                Utilities.Log(e.StackTrace);
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENTID");
                var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENTSECRET");
                var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANTID");
                var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud);

                var azure = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithDefaultSubscription();

                Utilities.Log("Selected subscription: " + azure.SubscriptionId);
                WalkCosmosAccounts(azure).Wait();
            }
            catch (Exception e)
            {
                Utilities.Log(e.Message);
                Utilities.Log(e.StackTrace);
            }
        }
    }
}
