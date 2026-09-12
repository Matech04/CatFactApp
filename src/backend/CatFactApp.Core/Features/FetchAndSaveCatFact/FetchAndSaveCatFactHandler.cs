using System.Net.Http.Json;
using CatFactFetcher.Core.Share.Storage;
using FluentResults;

namespace CatFactFetcher.Functions.Features.FetchAndSaveCatFact;

public class FetchAndSaveCatFactHandler(HttpClient _httpClient, IFileStorage _storage)
{
    public async Task<Result<CatFactDto>> HandleAsync(CancellationToken ct)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<CatFactDto>(new Uri("https://catfact.ninja/fact"), ct);

            if (result is null)
            {
                return Result.Fail("External server answered with empty fact");
            }

            string line = $"{DateTime.UtcNow} | {result.Fact} | {result.Length}";
            await _storage.SaveLineAsync(line, ct);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}