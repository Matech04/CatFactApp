using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CatFactFetcher.Functions.Features.GetStoredCatFacts;

public class GetStoredCatFactsFunction(ILogger<GetStoredCatFactsFunction> logger, GetStoredCatFactsHandler handler)
{
    [Function("GetStoredCatFacts")]
    public async Task<IResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cat-fact")] HttpRequest req, CancellationToken ct)
    {
        logger.LogInformation("Processing GetStoredCatFacts Functions request.");

        var result = await handler.HandleAsync(ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem();
    }
}