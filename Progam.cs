using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using ABCRetailApp_Functions.Services;
using System;
using Stripe.Climate;
using OrderService = ABCRetailApp_Functions.Services.OrderService;
using Order = ABCRetailApp_Functions.Models.Order;

[assembly: FunctionsStartup(typeof(ABCRetailApp_Functions.Program))]

namespace ABCRetailApp_Functions
{
    public class Program : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var containerName = "product"; // Set your container name
            var tableName = "Orders"; // Example table name for OrderService

            // BlobStorageService registration (with logger provided by DI)
            builder.Services.AddSingleton<BlobStorageService>(provider =>
                new BlobStorageService(connectionString, containerName, provider.GetRequiredService<ILogger<BlobStorageService>>()));

            // QueueStorageService registration (with logger provided by DI)
            builder.Services.AddSingleton<QueueStorageService>(provider =>
                new QueueStorageService(connectionString, "order", provider.GetRequiredService<ILogger<QueueStorageService>>()));

            // TableStorageService registration for Order (with logger provided by DI)
            builder.Services.AddSingleton<TableStorageService<Order>>(provider =>
                new TableStorageService<Order>(connectionString, tableName, provider.GetRequiredService<ILogger<TableStorageService<Order>>>()));

            // Register OrderService using TableStorageService
            builder.Services.AddSingleton<OrderService>(provider =>
                new OrderService(provider.GetRequiredService<TableStorageService<Order>>()));
        }
    }
}

