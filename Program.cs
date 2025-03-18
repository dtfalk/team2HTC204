using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Retrieve storage connection string from environment variables.
        string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;
        services.AddSingleton(new BlobServiceClient(storageConnectionString));
    })
    .Build();

host.Run();