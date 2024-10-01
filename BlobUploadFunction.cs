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
    public class BlobUploadFunction
    {
        private readonly BlobStorageService _blobStorageService;

        public BlobUploadFunction(BlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [FunctionName("UploadBlob")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "upload")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to upload a blob.");

            // Read the file from the request
            var file = req.Form.Files[0];
            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("No file uploaded.");
            }

            using (var stream = file.OpenReadStream())
            {
                await _blobStorageService.UploadBlobAsync(file.FileName, stream);
            }

            return new OkObjectResult($"Blob {file.FileName} uploaded successfully.");
        }
    }
}
