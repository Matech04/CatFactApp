namespace CatFactFetcher.Functions.Features.FetchAndSaveCatFact;

public static class CatFactEndpoints
{
    public static void MapFetchAndSaveCatFactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/cat-fact");
    
        group.MapPost("/", async (FetchAndSaveCatFactHandler handler, CancellationToken ct) => {
            
            var result = await handler.HandleAsync(ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem();
        })
        .RequireRateLimiting("fixed");
    }

}