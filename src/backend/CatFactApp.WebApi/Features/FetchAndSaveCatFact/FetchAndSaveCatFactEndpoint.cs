using CatFactApp.Core.Features.FetchAndSaveCatFact;
using CatFactApp.WebApi.Extensions;

namespace CatFactApp.WebApi.Features.FetchAndSaveCatFact;

public static class CatFactEndpoints
{
    public static void MapFetchAndSaveCatFactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/cat-fact");
    
        group.MapPost("/", async (FetchAndSaveCatFactHandler handler, CancellationToken ct) => {
            
            var result = await handler.HandleAsync(ct);
            return result.ToProblemDetails();
        })
        .RequireRateLimiting("fixed");
    }

}