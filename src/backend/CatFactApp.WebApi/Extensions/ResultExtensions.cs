using FluentResults;
using Microsoft.AspNetCore.Http;

namespace CatFactApp.WebApi.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        var firstError = result.Errors.FirstOrDefault();
        var errorMessage = firstError?.Message ?? "Wystąpił nieznany błąd.";

        int statusCode = StatusCodes.Status400BadRequest;
        if (firstError?.Metadata.TryGetValue("StatusCode", out var codeObj) == true && codeObj is int code)
        {
            statusCode = code;
        }

        return TypedResults.Problem(
            detail: errorMessage,
            statusCode: statusCode,
            title: GetTitleForStatusCode(statusCode)
        );
    }

    private static string GetTitleForStatusCode(int statusCode) => statusCode switch
    {
        StatusCodes.Status404NotFound => "Nie znaleziono zasobu",
        StatusCodes.Status400BadRequest => "Błąd żądania",
        StatusCodes.Status401Unauthorized => "Brak autoryzacji",
        StatusCodes.Status403Forbidden => "Brak dostępu",
        _ => "Wystąpił błąd serwera"
    };
}