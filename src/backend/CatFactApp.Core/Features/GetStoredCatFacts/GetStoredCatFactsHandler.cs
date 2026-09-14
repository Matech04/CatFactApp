namespace CatFactFetcher.Functions.Features.GetStoredCatFacts;

using CatFactFetcher.Core.Share.Storage;
using FluentResults;
using Microsoft.Extensions.Logging;
using CatFactFetcher.Core.Shared.Entities;
using Microsoft.Extensions.Caching.Memory;

public class GetStoredCatFactsHandler(ILogger<GetStoredCatFactsHandler> logger, IFileStorage storage, IMemoryCache cache)
{

    public const string CacheKey = "StoredCatFacts_List";

    public async Task<Result<CatFactRecord[]>> HandleAsync(CancellationToken ct)
    {

        if(cache.TryGetValue(CacheKey, out CatFactRecord[]? cachedRecords) && cachedRecords is not null)
        {
            logger.LogInformation("Returning cat facts from MemoryCache.");
            return Result.Ok(cachedRecords);
        }

        await using var fileStream = await storage.GetFileStreamAsync(ct);

        if(fileStream is null)
        {
            logger.LogWarning("File stream was null when attempting to read stored cat facts.");
            return Result.Fail("Error while trying to read File");
        }

        using var reader = new StreamReader(fileStream);

        var records = new List<CatFactRecord>();
        string? line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split("|").Select(p => p.Trim()).ToArray();

            if (parts.Length == 3 && DateTime.TryParse(parts[0], out var date ) && int.TryParse(parts[2], out var length))
            {
                records.Add(new CatFactRecord(date, parts[1], length));
            }
        }

        var resultRecords = records.ToArray();

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

        cache.Set(CacheKey, resultRecords, cacheOptions);
    
        logger.LogInformation("Returning cat facts from File Storage");
        return Result.Ok(resultRecords);
    }
}