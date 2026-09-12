using CatFactApp.Core;
using CatFactFetcher.Functions.Features.FetchAndSaveCatFact;
using CatFactFetcher.Functions.Features.GetStoredCatFacts;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices(builder.Configuration);

builder.Services.AddOpenApi();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddRateLimiter(options =>
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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseCors("AllowBlazor");
app.UseRateLimiter();

app.UseHttpsRedirection();

app.MapFetchAndSaveCatFactEndpoints();
app.MapGetStoredCatFactsEndpoints();

app.Run();


