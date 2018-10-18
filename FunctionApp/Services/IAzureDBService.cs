using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDbTrackerApp.Models;

namespace CosmosDbTrackerApp.Services
{
    public interface IAzureDBService
    {
        Task<IList<Cosmosdb>> GetCosmosAccountInfo();
        Task<Cosmosdb> AddNewCollectionRecord(Cosmosdb item);
        Task<Cosmosdb> UpdateCollectionRecord(string id, Cosmosdb item);
    }
}
