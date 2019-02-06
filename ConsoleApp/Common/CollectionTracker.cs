using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace bjd.CosmosDB.CollectionTracker
{
    public class Tracker
    {
        public List<CosmosDbAccounts> cosmosDbAccounts = new List<CosmosDbAccounts>();
        private string WriteDash()
        {
            int count = 65;
            return string.Concat(Enumerable.Repeat("-", count));
        }

        public void Print()
        {
            Console.WriteLine(this.WriteDash());
            Console.WriteLine("{0,-20}\t| {1,-20}\t| {2,-20}", "Account", "Database", "Collections");
            Console.WriteLine(this.WriteDash());
            cosmosDbAccounts.ForEach(act => act.Print());
            Console.WriteLine(this.WriteDash());
        }
    }

    public class CosmosDbAccounts
    {
        private List<CosmosDbCollection> _cosmosDbDatabaseCollections = new List<CosmosDbCollection>();
        public string MasterKey { get; set; }
        public string Accountname { get; set; }
        public string EndPoint { get; set; }
        private DocumentClient client;
        public async Task QueryCosmosDbAccountForDatabasesAndCollections()
        {
            using (client = new DocumentClient(new Uri(EndPoint), MasterKey, ConnectionPolicy.Default))
            {
                foreach (var database in await client.ReadDatabaseFeedAsync())
                {
                    foreach (var collection in client.CreateDocumentCollectionQuery(database.SelfLink))
                    {
                        _cosmosDbDatabaseCollections.Add(new CosmosDbCollection()
                        {
                            DatabaseName = database.Id,
                            CollectionName = collection.Id,
                        });
                    }
                }
            }
        }

        public void Print()
        {
            _cosmosDbDatabaseCollections.ForEach(col => Console.WriteLine($"{Accountname,-20}\t| {col.DatabaseName,-20}\t| {col.CollectionName,-20}"));
        }
    }
    public class CosmosDbCollection
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
