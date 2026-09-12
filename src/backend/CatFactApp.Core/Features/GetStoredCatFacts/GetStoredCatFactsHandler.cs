namespace CatFactFetcher.Functions.Features.GetStoredCatFacts;

using CatFactFetcher.Core.Share.Storage;
using FluentResults;
using Microsoft.Extensions.Logging;

public class GetStoredCatFactsHandler(ILogger<GetStoredCatFactsHandler> logger, IFileStorage storage)
{
    public async Task<Result<CatFactRecord[]>> HandleAsync(CancellationToken ct)
    {

        logger.LogWarning(storage.GetType().Name);

        await using var fileStream = await storage.GetFileStreamAsync(ct);

        if(fileStream is null)
        {
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
    
        return Result.Ok(records.ToArray());
    }
}