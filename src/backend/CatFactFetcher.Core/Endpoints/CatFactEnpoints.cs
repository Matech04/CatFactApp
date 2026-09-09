using CatFactFetcher.Core.Dto;
using CatFactFetcher.Core.Services;
using FluentResults;

namespace CatFactFetcher.Core.Endpoints;

public static class CatFactEndpoints
{
    public static void MapCatFactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/cat-fact");
    
        group.MapPost("/", FetchCatFact);
    }

    private static async Task<IResult> FetchCatFact(IFetchDataService fetchService, ISaveCatFactService saveService)
    {
        var result = await fetchService.FetchAsync<CatFactDto>("https://catfact.ninja/fact");

        if (result.IsSuccess)
        {
            await saveService.SaveAsync(result.Value.fact, result.Value.length);
        
            return Results.Ok();
        }

        return Results.BadRequest(new
        {
            errors = result.Errors.Select(e => e.Message)
        });
        
    }
}