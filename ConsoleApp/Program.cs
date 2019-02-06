using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

namespace bjd.CosmosDB.CollectionTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var tracker = new Tracker();
                var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENTID");
                var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENTSECRET");
                var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANTID");
                var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud);

                var azure = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithSubscription(Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTIONID"));

                Utilities.Log($"Selected subscription: {azure.SubscriptionId}");
                            
                foreach (var cosmosDBAccount in azure.CosmosDBAccounts.List())
                {
                    var account = new CosmosDbAccounts() {
                        Accountname = cosmosDBAccount.Name,
                        EndPoint = cosmosDBAccount.DocumentEndpoint,
                        MasterKey = cosmosDBAccount.ListKeys().PrimaryMasterKey
                    };
                    Task.Run( async () => { 
                        await account.QueryCosmosDbAccountForDatabasesAndCollections(); 
                    }).GetAwaiter().GetResult();
                    tracker.cosmosDbAccounts.Add(account);
                }
                tracker.Print(); 
            }
            catch (Exception e)
            {
                Utilities.Log(e.Message);
                Utilities.Log(e.StackTrace);
            }
        }
    }
}
