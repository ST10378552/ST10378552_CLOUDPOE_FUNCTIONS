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
    public static class OrderQueryFunction
    {
        public class OrderQueryFunctions
        {
            private readonly OrderService _orderService;
            private readonly ILogger<OrderQueryFunctions> _logger;

            public OrderQueryFunctions(OrderService orderService, ILogger<OrderQueryFunctions> logger)
            {
                _orderService = orderService;
                _logger = logger;
            }

            [FunctionName("GetAllOrders")]
            public async Task<IActionResult> GetAllOrders(
                [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/all")] HttpRequest req)
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return new OkObjectResult(orders);
            }
        }
    }
}
