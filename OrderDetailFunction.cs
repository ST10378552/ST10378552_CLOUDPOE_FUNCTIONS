using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ABCRetailApp_Functions.Services;

namespace ABCRetailApp_Functions
{
    public class OrderDetailFunction
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderDetailFunction> _logger;

        public OrderDetailFunction(OrderService orderService, ILogger<OrderDetailFunction> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [FunctionName("GetOrderById")]
        public async Task<IActionResult> GetOrderById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{partitionKey}/{rowKey}")] HttpRequest req,
            string partitionKey,
            string rowKey)
        {
            var order = await _orderService.GetOrderAsync(partitionKey, rowKey);
            return order != null ? new OkObjectResult(order) : new NotFoundResult();
        }
    }
}
