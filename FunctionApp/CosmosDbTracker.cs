using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbTrackerApp.Models;
using CosmosDbTrackerApp.Common;
using CosmosDbTrackerApp.Services;

namespace CosmosDbTrackerApp
{
    public static class CosmosDbTracker
    {
        public static async Task WalkCosmosAccounts(IAzure azure, ILogger log)
        {
            var dBService = new AzureDBService();
            var tableTracker = await dBService.GetCosmosAccountInfo();

            try
            {
				foreach (var cosmosDBAccount in azure.CosmosDBAccounts.List()) {
					var databaseAccountListKeysResult = cosmosDBAccount.ListKeys();
					string masterKey = databaseAccountListKeysResult.PrimaryMasterKey;
					string endPoint = cosmosDBAccount.DocumentEndpoint;

					var client = new DocumentClient(new Uri(endPoint), masterKey, ConnectionPolicy.Default);
					var databases = await client.ReadDatabaseFeedAsync();
						
					log.LogInformation($"Reading all databases resources for - {cosmosDBAccount.Name}");
					foreach (var db in databases) {
						List<DocumentCollection> collections = client.CreateDocumentCollectionQuery((String)db.SelfLink).ToList();
						foreach (var col in collections) {
                            var record = tableTracker
                                    .Where(x => x.AccountName == cosmosDBAccount.Name)
                                    .Where(x => x.DatabaseName == db.Id)
                                    .Where(x => x.CollectionName == col.Id).SingleOrDefault();
                            if( tableTracker == null || record == null ) {
                                await dBService.AddNewCollectionRecord( new Cosmosdb(){ AccountName = cosmosDBAccount.Name, DatabaseName = db.Id, CollectionName = col.Id }) ;
                            }
                            else {
                                await dBService.UpdateCollectionRecord( record.RowKey, record );
                            }
						}
					}
				}
            }
			catch (Exception e) {
				log.LogInformation(e.Message);
				log.LogInformation(e.StackTrace);
			}
		}

        [FunctionName("CosmosDbTracker")]
        public static void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            try
            {
                var spn = new AzureServicePrincipal();
				var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal( spn.clientId, spn.clientSecret, spn.tenantId, AzureEnvironment.AzureGlobalCloud );

				var azure = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithDefaultSubscription();

				WalkCosmosAccounts(azure, log).Wait();
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
                log.LogInformation(e.StackTrace);
            }
        }
    }
}
