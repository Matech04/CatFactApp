using CatFactFetcher.Core.Services;

namespace CatFactFetcher.Core.Endpoints;

public static class CatFactEndpoints
{
    public static void MapCatFactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/cat-fact");
    
        group.MapPost("/", FetchCatFact);
    }

    private static async Task<IResult> FetchCatFact(ISaveCatFactService saveService)
    {
        var result = await saveService.SaveAsync();

        if (result.IsSuccess)
        {
            return Results.Ok();
        }

        return Results.BadRequest(new
        {
            errors = result.Errors.Select(e => e.Message)
        });
        
    }
}