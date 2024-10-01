using Azure.Data.Tables;
using Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCRetailApp_Functions.Models;

namespace ABCRetailApp_Functions.Services
{
    public class OrderService
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<OrderService> _logger;
        private TableStorageService<Order> tableStorageService;

        public OrderService(TableStorageService<Order> tableStorageService)
        {
            this.tableStorageService = tableStorageService;
        }

        public OrderService(TableClient tableClient, ILogger<OrderService> logger)
        {
            _tableClient = tableClient;
            _logger = logger;
        }

        public async Task AddOrderAsync(Order order)
        {
            try
            {
                await _tableClient.AddEntityAsync(order);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error adding order to table storage.");
                throw new ApplicationException("Could not add order to table storage.", ex);
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();
            await foreach (var entity in _tableClient.QueryAsync<Order>())
            {
                orders.Add(entity);
            }
            return orders;
        }

        public async Task<Order> GetOrderAsync(string partitionKey, string rowKey)
        {
            try
            {
                // Retrieve the order entity from Azure Table Storage
                var response = await _tableClient.GetEntityAsync<Order>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Return null if the order is not found
                return null;
            }
        }
    }
}
