using CatFactApp.Core.Shared.Storage;
using CatFactFetcher.Core.Share.Storage;
using CatFactFetcher.Functions.Features.FetchAndSaveCatFact;
using CatFactFetcher.Functions.Features.GetStoredCatFacts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CatFactApp.Core;

public static class DependencyInjection
{

    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMemoryCache();

        services.AddHttpClient<FetchAndSaveCatFactHandler>().AddStandardResilienceHandler();

        services.AddScoped<GetStoredCatFactsHandler>();
        services.AddScoped<FetchAndSaveCatFactHandler>();

        services.Configure<FileSystemSettings>(configuration.GetSection("FileSystemSettings"));
        services.Configure<CatFactApiSettings>(configuration.GetSection("CatFactApiSettings"));

        services.AddKeyedScoped<IFileStorage, LocalStorage>(FileSystemType.Local);
        services.AddKeyedScoped<IFileStorage, AzureBlobStorage>(FileSystemType.AzureBlob);

        services.AddScoped<IFileStorage>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FileSystemSettings>>().Value;
            return sp.GetRequiredKeyedService<IFileStorage>(options.Provider);
        });

        return services;

    }

};

