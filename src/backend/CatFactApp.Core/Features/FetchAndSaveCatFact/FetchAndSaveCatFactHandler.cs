using System.Net.Http.Json;
using CatFactApp.Core.Features.GetStoredCatFacts;
using CatFactFetcher.Core.Share.Storage;
using CatFactFetcher.Core.Shared.Entities;
using FluentResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CatFactApp.Core.Features.FetchAndSaveCatFact;

public class FetchAndSaveCatFactHandler(HttpClient httpClient, IFileStorage storage, IMemoryCache cache, ILogger<FetchAndSaveCatFactHandler> logger)
{
    public async Task<Result<CatFactRecord>> HandleAsync(CancellationToken ct)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<CatFactDto>(new Uri("https://catfact.ninja/fact"), ct);

            if (result is null)
            {
                return Result.Fail("External server answered with empty fact");
            }

            string line = $"{DateTime.UtcNow} | {result.Fact} | {result.Length}";
            await storage.SaveLineAsync(line, ct);

            cache.Remove(GetStoredCatFactsHandler.CacheKey);

            return Result.Ok(new CatFactRecord(DateTime.UtcNow, result.Fact, result.Length));
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Exception while adding fact to list: {ex.Message}");
            return Result.Fail(ex.Message);
        }
    }
}