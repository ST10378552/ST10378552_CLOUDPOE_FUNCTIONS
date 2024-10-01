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

namespace ABCRetailApp_Functions
{
    public static class StoreCustomerProfilesFunction
    {
        [FunctionName("StoreCustomerProfile")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("Processing a request to store customer profile.");

            // Read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CustomerProfile customer = JsonConvert.DeserializeObject<CustomerProfile>(requestBody);

            // Validate customer data
            if (customer == null || string.IsNullOrEmpty(customer.Email))
            {
                return new BadRequestObjectResult("Invalid customer data.");
            }

            // Define connection string and table name
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string tableName = "Customers";

            // Use the service to store customer data in Azure Tables
            var tableStorageService = new TableStorageService<CustomerProfile>(storageConnectionString, tableName);
            await tableStorageService.InsertOrMergeEntityAsync(customer);

            return new OkObjectResult($"Customer {customer.Email} has been successfully stored.");
        }
    }
}
