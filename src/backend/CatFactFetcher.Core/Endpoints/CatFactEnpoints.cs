using CatFactFetcher.Core.Dto;
using CatFactFetcher.Core.Services;

namespace CatFactFetcher.Core.Endpoints;

public static class CatFactEndpoints
{
    public static void MapCatFactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/cat-fact");
    
        group.MapPost("/", FetchCatFact);
    }

    private static async Task<IResult> FetchCatFact(IFetchDataService service)
    {
        var result = await service.FetchAsync<CatFactDto>("https://catfact.ninja/fact");

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return Results.BadRequest(new
        {
            errors = result.Errors.Select(e => e.Message)
        });
        
    }
}