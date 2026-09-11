namespace CatFactFetcher.Functions.Features.GetStoredCatFactsEndpoint;

public static class CatFactEndpoints
{
    public static void MapGetStoredCatFactsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("cat-fact");

        group.MapGet("/", async (GetStoredCatFactsHandler handler, CancellationToken ct) =>
        {

            var result = await handler.HandleAsync(ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem();
        })
        .RequireRateLimiting("fixed");
    }
}