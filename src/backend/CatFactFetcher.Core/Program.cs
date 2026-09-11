using CatFactFetcher.Core.Endpoints;
using CatFactFetcher.Core.Services;
using CatFactFetcher.Core.Storages;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IFetchDataService, FetchDataService>();
builder.Services.AddSingleton<ISaveCatFactService, SaveCatFactService>();

builder.Services.AddKeyedScoped<IFileSystemRepository, LocalFileRepository>(FileSystemType.Local);

builder.Services.AddScoped<IFileSystemRepository>(sp =>
{
    var options = sp.GetRequiredService<IOptions<FileSystemSettings>>().Value;
    return sp.GetRequiredKeyedService<IFileSystemRepository>(options.Provider);
});


var app = builder.Build();


    app.MapOpenApi();
    app.MapScalarApiReference();

app.MapCatFactEndpoints();

app.Run();
