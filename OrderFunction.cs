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
using ABCRetailApp_Functions.Models;

namespace ABCRetailApp_Functions
{
    public class OrderFunctions
    {
        private readonly QueueStorageService _queueService;
        private readonly ILogger<OrderFunctions> _logger;

        public OrderFunctions(QueueStorageService queueService, ILogger<OrderFunctions> logger)
        {
            _queueService = queueService;
            _logger = logger;
        }

        [FunctionName("SendOrderToQueue")]
        public async Task<IActionResult> SendOrderToQueue(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders/send")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            if (order == null)
            {
                return new BadRequestObjectResult("Invalid order data.");
            }

            await _queueService.SendMessageAsync(JsonConvert.SerializeObject(order));
            return new OkResult();
        }
    }
}
