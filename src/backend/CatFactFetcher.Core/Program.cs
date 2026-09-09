using CatFactFetcher.Core.Endpoints;
using CatFactFetcher.Core.Services;
using CatFactFetcher.Core.Storages;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IFetchDataService, FetchDataService>();

builder.Services.AddSingleton<ISaveCatFactService, SaveCatFactService>();
builder.Services.AddSingleton<ICatFactRepository, CatFactRepository>();


var app = builder.Build();


    app.MapOpenApi();
    app.MapScalarApiReference();

app.MapCatFactEndpoints();

app.Run();
