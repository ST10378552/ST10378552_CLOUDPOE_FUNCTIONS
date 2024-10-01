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
    public static class StoreProductsFunction
    {
        [FunctionName("StoreProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing a request to store product.");

            // Read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(requestBody);

            // Validate product data
            if (product == null || string.IsNullOrEmpty(product.ProductName))
            {
                return new BadRequestObjectResult("Invalid product data.");
            }

            // Define connection string and table name
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string tableName = "Products";

            // Use the service to store product data in Azure Tables
            var tableStorageService = new TableStorageService<Product>(storageConnectionString, tableName);
            product.PartitionKey = "Product"; // Set the partition key
            product.RowKey = Guid.NewGuid().ToString(); // Set a unique row key

            try
            {
                await tableStorageService.InsertOrMergeEntityAsync(product);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error saving product: {ErrorMessage}", ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult($"Product '{product.ProductName}' has been successfully stored.");
        }
    }
}
