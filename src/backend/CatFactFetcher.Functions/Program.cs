using CatFactFetcher.Core.Storages;
using CatFactFetcher.Functions.Features.FetchAndSaveCatFact;
using CatFactFetcher.Functions.Features.GetStoredCatFactsEndpoint;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.Services.AddHttpClient<FetchAndSaveCatFactHandler>()
.AddStandardResilienceHandler();

builder.Services.AddRateLimiter( options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter(policyName: "fixed", fixedOtions =>
    {
        fixedOtions.PermitLimit = 10;
        fixedOtions.Window = TimeSpan.FromSeconds(10);
        fixedOtions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        fixedOtions.QueueLimit = 2;
    });
});

builder.Services.AddScoped<GetStoredCatFactsHandler>();
builder.Services.AddScoped<FetchAndSaveCatFactHandler>();

builder.Services.Configure<FileSystemSettings>(builder.Configuration.GetSection("FileSystemSettings"));
builder.Services.Configure<CatFactApiSettings>(builder.Configuration.GetSection("CatFactApiSettings"));

builder.Services.AddKeyedScoped<IFileStorage, LocalStorage>(FileSystemType.Local);

builder.Services.AddScoped<IFileStorage>(sp =>
{
    var options = sp.GetRequiredService<IOptions<FileSystemSettings>>().Value;
    return sp.GetRequiredKeyedService<IFileStorage>(options.Provider);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();
app.UseCors("AllowBlazor");
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapFetchAndSaveCatFactEndpoints();
app.MapGetStoredCatFactsEndpoints();

app.Run();
