using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Worker;
using Services.Interfaces;

namespace AzureFunctions
{
    //Example function class that is triggered when the blob storage is changed (add, update). This is purely to showcase how our service layer can be used to interact with our functions. 
    //We have logical seperation of function via different classes in our AzureFunction project. We could put them all in one class if we like.
    public class BlobFunction
    {
        private readonly ILogger _logger;
        private ISomeProductService _someProductService;
        private IConfiguration _configuration;

        public BlobFunction(ISomeProductService ISomeProductService, ILogger<BlobFunction> logger, IConfiguration configuration)
        {
            _someProductService = ISomeProductService;
            _logger = logger;
            _configuration = configuration;
        }

        //This would be called whenever a Blob is added to our blob container
        [Function("DownloadBlob")]
        public async Task Parse([BlobTrigger("SomeBlobContainer/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, Uri uri)
        {
            _logger.LogInformation($"C# Blob trigger function Processing blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var success = await _someProductService.DownloadBlobAsync(uri.ToString(), name, new CancellationToken());

            if (success)
                _logger.LogInformation($"C# Blob trigger function Processed blob\n Name:{name}");
            
            else
                _logger.LogInformation($"C# Blob trigger function failed to delete blob\n Name:{name}");
        }
    }
}
