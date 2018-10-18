using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDbTrackerApp.Models;
using Microsoft.Extensions.Configuration;
using CosmosDbTrackerApp.Common;

namespace CosmosDbTrackerApp.Services
{
	public class AzureDBService : IAzureDBService
	{
		private static AzureTableRepository<Cosmosdb> _tableProvider;

		public AzureDBService()
		{
			_tableProvider = new AzureTableRepository<Cosmosdb>(new AzureTableSettings());
		}

		public async Task<IList<Cosmosdb>> GetCosmosAccountInfo()
		{
			return await _tableProvider.GetItems();		
		}

		public async Task<Cosmosdb> AddNewCollectionRecord(Cosmosdb item)
		{
			item.FirstSeenDate = item.LastUpdatedDate = DateTime.Now;
			return await _tableProvider.CreateItemAsync(item);
		}

		public async Task<Cosmosdb> UpdateCollectionRecord(string id, Cosmosdb item)
		{
			if (id != item.RowKey)
			{
				return null;
			}

			item.LastUpdatedDate = DateTime.Now;
			return await _tableProvider.UpdateItemAsync(id, item);
		}
	}
}
