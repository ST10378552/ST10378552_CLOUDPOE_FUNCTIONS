using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Files.Shares;

namespace ABCRetailApp_Functions
{
    public class FIleFunction
    {
        private readonly string _storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private readonly string _shareName = "contract-logs";

        [FunctionName("UploadFile")]
        public async Task<IActionResult> UploadFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var file = req.Form.Files[0];
            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("No file uploaded.");
            }

            var shareClient = new ShareClient(_storageConnectionString, _shareName);
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await fileClient.CreateAsync(stream.Length);
                await fileClient.UploadAsync(stream);
            }

            return new OkObjectResult($"File {file.FileName} uploaded successfully.");
        }
    }
}
