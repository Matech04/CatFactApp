using Azure.Monitor.OpenTelemetry.Exporter;
using Azure.Storage.Blobs;
using CatFactApp.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

var connectionString = builder.Configuration["CustomBlobStorageConnection"] ?? "UseDevelopmentStorage = true";

builder.Services.AddSingleton(new BlobServiceClient(connectionString));

builder.Services.AddCoreServices(builder.Configuration);

builder.ConfigureFunctionsWebApplication();


if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
