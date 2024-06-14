using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace AzureFunctions
{
    //Example function class that can perform updates to DB. This is purely to showcase how our service layer can interact with our functions. 
    //We have logical seperation of function via different classes in our AzureFunction project. We could put them all in one class if we like.
    public class ProductFunction
    {
        private readonly ILogger _logger;
        private readonly IRelationalService _relationalService;


        public ProductFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProductFunction>();
        }

        //Example timer trigger using Cron expression to call some stored procedure in database.
        [Function("UpdateProductTable")]
        public async Task RunAsync([TimerTrigger("0 */6 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation("UpdateProductTable service started: {start}", DateTime.Now);
            await _relationalService.UpdateTableViaSPAsync();
            _logger.LogInformation("UpdateProductTable service completed at {finish}", DateTime.Now);
        }
    }
}
