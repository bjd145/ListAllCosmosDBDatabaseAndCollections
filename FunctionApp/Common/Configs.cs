using System;
using Microsoft.Extensions.Configuration;

namespace CosmosDbTrackerApp.Common
{
    public class AzureTableSettings
    {
        public string connectionString { get; private set; }
        public string tableName { get; private set; }
        public string partitionKey { get; private set; }

        public AzureTableSettings()
        {
            connectionString = Environment.GetEnvironmentVariable("TABLE_CONSTR");
            tableName = Environment.GetEnvironmentVariable("TABLE_NAME"); ;
            partitionKey = Environment.GetEnvironmentVariable("TABLE_PARTITIONKEY");
        }
    }
    public class AzureServicePrincipal
    {
        public string clientId { get; private set; }
        public string clientSecret { get; private set; }
        public string tenantId { get; private set; }

        public AzureServicePrincipal()
        {
            clientId = Environment.GetEnvironmentVariable("AZURE_CLIENTID");
            clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENTSECRET");
            tenantId = Environment.GetEnvironmentVariable("AZURE_TENANTID");
        }
    }

}
