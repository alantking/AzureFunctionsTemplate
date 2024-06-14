using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Database;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Services.Interfaces;
using Services.Services;

//Make sure there are values in config file, or app will crash
var storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
var connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
var requestTimeout = TimeSpan.FromMilliseconds(Environment.GetEnvironmentVariable("RequestTimeout") != null ? Convert.ToDouble(Environment.GetEnvironmentVariable("RequestTimeout")) : 5000);
var blobClientOptions = new BlobClientOptions { Retry = { Delay = TimeSpan.FromSeconds(2), MaxRetries = 3, Mode = RetryMode.Exponential } };
var queueClientOptions = new QueueClientOptions { Retry = { Delay = TimeSpan.FromSeconds(2), MaxRetries = 3, Mode = RetryMode.Exponential }, MessageEncoding = QueueMessageEncoding.Base64 };

var builder = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    });

//Configure our services for dependency injection
builder.ConfigureServices(services =>
{
    Log.Logger = new LoggerConfiguration()
       .WriteTo.Console()
               .CreateLogger();
    services.AddSingleton<ILoggerProvider>(sp => new SerilogLoggerProvider(Log.Logger, true));
    services.AddLogging(opt =>
    {
        opt.AddSerilog(Log.Logger, true);
    });

    //This allows us to create http requests (ex: api requests) by injecting httpclientfactory in our actual service.
    services.AddHttpClient();
    //Example services we have creeated.
    services.AddTransient<IRelationalDataService, RelationalDataService>();
    services.AddTransient<IRelationalService, RelationalService>();
    services.AddTransient<ISomeProductDataService, SomeProductDataService>();
    services.AddTransient<ISomeProductService, SomeProductService>();

    //For Entity Framework, allows connection to db
    services.AddDbContext<SomeDBContext>(options =>
    {
        options.UseSqlServer(connectionString, cfg =>
        {
            cfg.EnableRetryOnFailure();
            cfg.CommandTimeout(60);
        });
    });

    //Connection to our storage account
    services.AddTransient<IQueueService, QueueService>(_ => new QueueService(new QueueServiceClient(storageConnectionString, queueClientOptions)));
    services.AddTransient<IBlobService, BlobService>(_ => new BlobService(new BlobServiceClient(storageConnectionString, blobClientOptions)));
}
);

//here we are using program.cs class to run our function app. It allows for more configuration of settings opposed to using a startup.cs class. 
//Using startup.cs is totally fine too, but you must inherit from FunctionsStartUp and include an assembly attribute.

var host = builder.Build();
host.Run();
