using CatFactApp.Core.Features.FetchAndSaveCatFact;
using CatFactApp.Functions.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CatFactFetcher.Functions.Features.FetchAndSaveCatFact;

public class FetchAndSaveCatFactFunction(ILogger<FetchAndSaveCatFactFunction> logger, FetchAndSaveCatFactHandler handler)
{
    [Function("FetchAndSaveCatFact")]
    public async Task<IResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cat-fact")] HttpRequest req, CancellationToken ct)
    {
        logger.LogInformation("Processing FetchAndSaveCatFact Functions request.");

        var result = await handler.HandleAsync(ct);

        return result.ToProblemDetails();
    }
}