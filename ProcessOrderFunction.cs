using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ABCRetailApp_Functions.Models;
using ABCRetailApp_Functions.Services;
using System.Collections.Generic;
using Stripe.Climate;
using Order = ABCRetailApp_Functions.Models.Order;
using OrderService = ABCRetailApp_Functions.Services.OrderService;

namespace ABCRetailApp_Functions
{
    public class ProcessOrderFunction
    {
        private readonly QueueStorageService _queueService;
        private readonly OrderService _orderService;
        private readonly ILogger<ProcessOrderFunction> _logger;

        // Constructor for DI
        public ProcessOrderFunction(QueueStorageService queueService, OrderService orderService, ILogger<ProcessOrderFunction> logger)
        {
            _queueService = queueService;
            _orderService = orderService;
            _logger = logger;
        }

        [FunctionName("ProcessOrdersFromQueue")]
        public async Task<IActionResult> ProcessOrdersFromQueue(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/process")] HttpRequest req)
        {
            var messages = await _queueService.ReceiveMessagesAsync(); // Get messages from Queue

            var orders = new List<Order>();
            foreach (var message in messages)
            {
                var order = JsonConvert.DeserializeObject<Order>(message.MessageText); // Deserialize message to Order
                if (order != null)
                {
                    await _orderService.AddOrderAsync(order); // Add order to Table Storage (via OrderService)
                    await _queueService.DeleteMessageAsync(message.MessageId, message.PopReceipt); // Delete processed message
                    orders.Add(order);
                }
            }

            return new OkObjectResult(orders); // Return list of processed orders
        }
    }
}
