using System.Text.Json.Serialization;

public record CatFactDto(
    [property: JsonPropertyName("fact")] string Fact,
    [property: JsonPropertyName("length")] int Length
);