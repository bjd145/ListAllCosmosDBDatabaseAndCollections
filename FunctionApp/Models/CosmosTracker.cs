using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table.Protocol;

using Microsoft.WindowsAzure.Storage.Table;

namespace CosmosDbTrackerApp.Models
{
    public class Cosmosdb : TableEntity
    {
        public virtual string AccountName { get; set; }
        public virtual string DatabaseName { get; set; }

        public virtual string CollectionName { get; set; }

        public virtual DateTime FirstSeenDate { get; set; }
        public virtual DateTime LastUpdatedDate { get; set; }

        public Cosmosdb()
        {
            this.RowKey = Guid.NewGuid().ToString();
        }
    }
}