using System.Text.Json;
using FluentResults;

namespace CatFactFetcher.Core.Services;

public interface IFetchDataService
{
    public  Task<Result<T>> FetchAsync<T>(string uri);
}

public class FetchDataService : IFetchDataService
{

    private readonly HttpClient _httpClient;

    public FetchDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<T>> FetchAsync<T>(string uri)
    {

        try
        {
            using var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<T>($"External Api Error: {(int)response.StatusCode}");
            }

            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType != "application/json" && mediaType?.EndsWith("+json") != true)
            {
                return Result.Fail<T>("Incorrect Content Type");
            }

            var data = await response.Content.ReadFromJsonAsync<T>();
            if (data == null)
            {
                return Result.Fail<T>("Json content is empty");
            }

            return Result.Ok(data);

        }
        catch (JsonException ex)
        {
            return Result.Fail<T>(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return Result.Fail<T>($"External Api Error: {ex.Message}");
        }

    }
}