using CatFactFetcher.Core.Endpoints;
using CatFactFetcher.Core.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IFetchDataService, FetchDataService>();

var app = builder.Build();


    app.MapOpenApi();
    app.MapScalarApiReference();

app.MapCatFactEndpoints();

app.Run();
