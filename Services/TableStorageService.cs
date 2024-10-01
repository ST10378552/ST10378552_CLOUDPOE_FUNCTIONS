using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCRetailApp_Functions.Services
{
    public class TableStorageService<T> where T : class, ITableEntity, new()
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<TableStorageService<T>> _logger;
        private string storageConnectionString;
        private string tableName;

        public TableStorageService(string storageConnectionString, string tableName)
        {
            this.storageConnectionString = storageConnectionString;
            this.tableName = tableName;
        }

        public TableStorageService(string connectionString, string tableName, ILogger<TableStorageService<T>> logger)
        {
            _tableClient = new TableClient(connectionString, tableName);
            _tableClient.CreateIfNotExists(); // Creates the table if it doesn't exist
            _logger = logger;
        }

        public async Task<T> GetEntityAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                _logger.LogWarning($"Entity with PartitionKey: {partitionKey} and RowKey: {rowKey} not found.");
                return null;
            }
        }



        public async Task InsertOrMergeEntityAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            await _tableClient.UpsertEntityAsync(entity);
        }
    }
}
